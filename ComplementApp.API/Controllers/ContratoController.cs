using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        #region Variable
        int usuarioId = 0;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ICDPRepository _cdpRepo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly IContratoRepository _repo;
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;

        #endregion Dependency Injection

        public ContratoController(IContratoRepository repo, IMapper mapper, ICDPRepository cdpRepo,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IProcesoCreacionArchivoExcel procesoCreacionExcelInterface )
        {
            _mapper = mapper;
            _repo = repo;
            _cdpRepo = cdpRepo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _procesoCreacionExcelInterface = procesoCreacionExcelInterface;
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaContrato([FromQuery(Name = "tipo")] int tipo,
                                                                        [FromQuery(Name = "terceroId")] int? terceroId,
                                                                        [FromQuery] UserParams userParams)
        {
            try
            {
                PagedList<CDPDto> pagedList = null;

                //Registrar contratos
                if (tipo == 1)
                {
                    pagedList = await _repo.ObtenerCompromisosSinContrato(terceroId, userParams);
                }
                else
                {
                    //Modificar contratos
                    pagedList = await _repo.ObtenerCompromisosConContrato(terceroId, userParams);
                }
                var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo obtener la lista de compromisos");
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerContrato([FromQuery(Name = "contratoId")] int contratoId)
        {
            try
            {
                var item = await _repo.ObtenerContrato(contratoId);
                return Ok(item);
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo obtener información del contrato");
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarContrato(ContratoDto contratoDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Contrato contrato = new Contrato();

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (contratoDto != null)
                {
                    var cdp = await _cdpRepo.ObtenerCDPPorCompromiso(contratoDto.Crp);

                    #region Mapear datos Contrato

                    contrato.NumeroContrato = contratoDto.NumeroContrato;
                    contrato.TipoContratoId = contratoDto.TipoContratoId;
                    contrato.Crp = contratoDto.Crp;
                    contrato.FechaRegistro = cdp.Fecha;
                    contrato.FechaExpedicionPoliza = contratoDto.FechaExpedicionPoliza;
                    contrato.FechaInicio = contratoDto.FechaInicio;
                    contrato.FechaFinal = contratoDto.FechaFinal;
                    contrato.Supervisor1Id = contratoDto.Supervisor1Id;
                    contrato.Supervisor2Id = contratoDto.Supervisor2Id.HasValue ? contratoDto.Supervisor2Id.Value : null;

                    contrato.UsuarioIdRegistro = usuarioId;
                    contrato.FechaInsercion = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos Contrato

                    //Registrar Parametro liquidación Contrato
                    _dataContext.Contrato.Add(contrato);
                    await _dataContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(contrato.ContratoId);
                }

            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo registrar el tercero");
        }

        [Route("[action]")]
        public async Task<IActionResult> ActualizarContrato(ContratoDto contratoDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var contratoBD = await _repo.ObtenerContratoBase(contratoDto.ContratoId);

                #region Mapear datos Contrato

                contratoBD.NumeroContrato = contratoDto.NumeroContrato;
                contratoBD.TipoContratoId = contratoDto.TipoContratoId;
                contratoBD.Crp = contratoDto.Crp;
                contratoBD.FechaExpedicionPoliza = contratoDto.FechaExpedicionPoliza;
                contratoBD.FechaInicio = contratoDto.FechaInicio;
                contratoBD.FechaFinal = contratoDto.FechaFinal;
                contratoBD.Supervisor1Id = contratoDto.Supervisor1Id;
                contratoBD.Supervisor2Id = contratoDto.Supervisor2Id;

                contratoBD.UsuarioIdModificacion = usuarioId;
                contratoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                #endregion Mapear datos Contrato

                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaContratoTotal()
        {
            string nombreArchivo = "Contrato.xlsx";
            try
            {
                var lista = await _repo.ObtenerListaContratoTotal();
                if (lista != null)
                {
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaContrato(lista.ToList());
                    return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest();
        }

    }
}
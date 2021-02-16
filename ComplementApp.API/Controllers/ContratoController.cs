using System;
using System.Collections.Generic;
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

        #endregion Dependency Injection

        public ContratoController(IContratoRepository repo, IMapper mapper, ICDPRepository cdpRepo,
                            DataContext dataContext, IGeneralInterface generalInterface)
        {
            _mapper = mapper;
            _repo = repo;
            _cdpRepo = cdpRepo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaContrato([FromQuery(Name = "tipo")] int tipo,
                                                                        [FromQuery(Name = "contratoId")] int? contratoId,
                                                                        [FromQuery] UserParams userParams)
        {
            try
            {
                PagedList<CDPDto> pagedList = null;

                if (tipo == 1)
                {
                    pagedList = await _repo.ObtenerCompromisosSinContrato(contratoId, userParams);
                }
                else
                {
                    pagedList = await _repo.ObtenerCompromisosConContrato(contratoId, userParams);
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
/*
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
                    #region Mapear datos Contrato

                    tercero.TipoIdentificacion = terceroDto.TipoDocumentoIdentidadId;
                    tercero.NumeroIdentificacion = terceroDto.NumeroIdentificacion;
                    tercero.Nombre = terceroDto.Nombre;
                    tercero.FechaExpedicionDocumento = terceroDto.FechaExpedicionDocumento;
                    tercero.Telefono = terceroDto.Telefono;
                    tercero.Email = terceroDto.Email;
                    tercero.Direccion = terceroDto.Direccion;
                    tercero.FacturadorElectronico = terceroDto.FacturadorElectronico;
                    tercero.DeclaranteRenta = terceroDto.DeclaranteRenta;

                    if (terceroDto.FacturadorElectronico)
                    {
                        tercero.RegimenTributario = "RESPONSABLE DE IVA";
                    }
                    else
                    {
                        tercero.RegimenTributario = "NO RESPONSABLE DE IVA";
                    }

                    tercero.UsuarioIdRegistro = usuarioId;
                    tercero.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos Contrato

                    //Registrar Parametro liquidación Contrato
                    _dataContext.Contrato.Add(tercero);
                    await _dataContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(tercero.ContratoId);
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
                //Registrar Contrato
                var terceroBD = await _repo.ObtenerContratoBase(terceroDto.ContratoId);

                #region Mapear datos Contrato

                terceroBD.TipoIdentificacion = terceroDto.TipoDocumentoIdentidadId;
                terceroBD.NumeroIdentificacion = terceroDto.NumeroIdentificacion;
                terceroBD.Nombre = terceroDto.Nombre;
                terceroBD.FechaExpedicionDocumento = terceroDto.FechaExpedicionDocumento;
                terceroBD.Telefono = terceroDto.Telefono;
                terceroBD.Email = terceroDto.Email;
                terceroBD.Direccion = terceroDto.Direccion;
                terceroBD.FacturadorElectronico = terceroDto.FacturadorElectronico;
                terceroBD.DeclaranteRenta = terceroDto.DeclaranteRenta;

                if (terceroDto.FacturadorElectronico)
                {
                    terceroBD.RegimenTributario = "RESPONSABLE DE IVA";
                }
                else
                {
                    terceroBD.RegimenTributario = "NO RESPONSABLE DE IVA";
                }

                terceroBD.UsuarioIdModificacion = usuarioId;
                terceroBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

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
*/
    }
}
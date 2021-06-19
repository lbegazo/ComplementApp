using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    
    public class CDPController : BaseApiController
    {
        #region Variable

        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ICDPRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGeneralInterface _generalInterface;
        public IUsuarioRepository UsuarioRepo => _usuarioRepo;
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;

        #endregion Dependency Injection

        public CDPController(ICDPRepository repo, IUsuarioRepository usuarioRepo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IProcesoCreacionArchivoExcel procesoCreacionExcelInterface)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _procesoCreacionExcelInterface = procesoCreacionExcelInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCompromiso([FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerListaCompromiso(userParams);
            var listaDto = _mapper.Map<IEnumerable<CDP>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]/{crp}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerRubrosPresupuestalesPorCompromiso(int crp)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            var rubros = await _repo.ObtenerRubrosPresupuestalesPorCompromiso(crp, pciId);
            return Ok(rubros);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePlanAnualAdquisicion([FromQuery] long cdp,
                                                                            [FromQuery] int instancia,
                                                                            [FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerDetallePlanAnualAdquisicion(cdp, instancia, userParams);
            var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarDetallePlanAnualAdquisicion([FromQuery(Name = "cdp")] int cdp,
                                                                                [FromQuery(Name = "instancia")] int instancia)
        {
            string tipoDocumento = string.Empty;

            var enumTipoDocumento = (TipoDocumento)instancia;
            tipoDocumento = enumTipoDocumento.ToString();

            string nombreArchivo = "DetallePlanAnualAdquisicion_" + tipoDocumento + "_" + cdp.ToString() + "_.xlsx";
            List<CDPDto> lista = null;
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                UserParams userParams = new UserParams();
                userParams.PageSize = 500;
                userParams.PageNumber = 1;
                userParams.PciId = pciId;

                var pagedList = await _repo.ObtenerDetallePlanAnualAdquisicion(cdp, instancia, userParams);
                if (pagedList != null)
                {
                    lista = _mapper.Map<List<CDPDto>>(pagedList);
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDetallePlanAnualAdquisicion(lista);
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
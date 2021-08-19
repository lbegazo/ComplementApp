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
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{

    public class PlanAdquisicionController : BaseApiController
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;
        int transaccionId = 71;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IPlanAdquisicionRepository _repo;
        private readonly IActividadGeneralRepository _repoActividad;
        private readonly IActividadGeneralService _serviceActividad;
        private readonly IListaRepository _repoLista;
        private readonly IMapper _mapper;
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;
        private readonly IPlanAdquisicionService _planAdquisicionService;

        #endregion Dependency Injection

        public PlanAdquisicionController(IPlanAdquisicionRepository repo,
                                    IActividadGeneralRepository repoActividad,
                                    IActividadGeneralService serviceActividad,
                                    IListaRepository repoLista,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IMapper mapper,
                                    IProcesoCreacionArchivoExcel procesoCreacionExcelInterface,
                                    IPlanAdquisicionService planAdquisicionService)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _repoActividad = repoActividad;
            _serviceActividad = serviceActividad;
            _repoLista = repoLista;
            _procesoCreacionExcelInterface = procesoCreacionExcelInterface;
            _planAdquisicionService = planAdquisicionService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerListaPlanAnualAdquisicion()
        {
            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                var listaDto = await _repo.ObtenerListaPlanAnualAdquisicion(pciId, usuarioId);
                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista del plan anual de adquisiciones");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegistrarPlanAdquisicion(PlanAdquisicion planAdquisicion)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            if (planAdquisicion != null)
            {
                planAdquisicion.UsuarioIdRegistro = usuarioId;
                await _planAdquisicionService.ActualizarPlanAdquisicion(pciId, transaccionId, planAdquisicion);

                return Ok(1);
            }
            return NoContent();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanAnualAdquisicionPaginada([FromQuery(Name = "esCreacion")] int esCreacion,
                                                                                   [FromQuery(Name = "rubroPresupuestalId")] int? rubroPresupuestalId,
                                                                                   [FromQuery(Name = "numeroCdp")] int? numeroCdp,
                                                                                   [FromQuery] UserParams userParams)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerListaPlanAnualAdquisicionPaginada(usuarioId, esCreacion, rubroPresupuestalId, numeroCdp, userParams);

            var listaDto = _mapper.Map<IEnumerable<DetalleCDPDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);
            return Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanAdquisicionSinCDPXIds([FromQuery(Name = "listaPlanAdquisicionId")] string listaPlanAdquisicionId,
                                                                                [FromQuery(Name = "seleccionarTodo")] int? seleccionarTodo,
                                                                                [FromQuery(Name = "rubroPresupuestalId")] int? rubroPresupuestalId)
        {
            List<DetalleCDPDto> listaFinal = new List<DetalleCDPDto>();
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            bool esSeleccionarTodo = seleccionarTodo > 0 ? true : false;

            if (esSeleccionarTodo)
            {
                #region esSeleccionarTodo

                UserParams userParams = new UserParams();
                userParams.PageSize = 500;
                userParams.PageNumber = 1;
                userParams.PciId = pciId;

                var pagedList = await _repo.ObtenerListaPlanAnualAdquisicionSinCDP(usuarioId, rubroPresupuestalId, userParams);
                listaFinal = _mapper.Map<List<DetalleCDPDto>>(pagedList);

                #endregion esSeleccionarTodo
            }
            else
            {
                #region Procesar por lista de ids

                if (!string.IsNullOrEmpty(listaPlanAdquisicionId))
                {
                    List<int> listaIds = listaPlanAdquisicionId.Split(',').Select(int.Parse).ToList();
                    listaFinal = await _repo.ObtenerListaPlanAdquisicionSinCDPXIds(listaIds);
                }

                #endregion Procesar por lista de ids
            }

            return Ok(listaFinal);

            throw new Exception($"No se puede obtener la lista de plan de adquisición");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanAdquisicionReporte([FromQuery(Name = "rubroPresupuestalId")] int? rubroPresupuestalId,
                                                                            [FromQuery] UserParams userParams)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerListaPlanAdquisicionReporte(usuarioId, rubroPresupuestalId, userParams);

            var listaDto = _mapper.Map<IEnumerable<DetalleCDPDto>>(pagedList);

            foreach (var item in listaDto)
            {
                var listaPlanHistorico = _dataContext.PlanAdquisicionHistorico.Where(x => x.PlanAdquisicioId == item.DetalleCdpId).ToList();
                if (listaPlanHistorico != null)
                {
                    var planHistoricoInicial = listaPlanHistorico.OrderBy(x => x.PlanAdquisicionHistoricoId).FirstOrDefault();
                    if (planHistoricoInicial != null)
                    {
                        item.ValorAct = planHistoricoInicial.Valor;
                    }
                }
            }

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);
            return Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaPlanAnualAdquisicion([FromQuery(Name = "rubroPresupuestalId")] int? rubroPresupuestalId)
        {
            string nombreArchivo = "PlanAnualAdquisicion.xlsx";
            List<DetalleCDPDto> lista = null;
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                UserParams userParams = new UserParams();
                userParams.PageSize = 5000;
                userParams.PageNumber = 1;
                userParams.PciId = pciId;

                var pagedList = await _repo.ObtenerListaPlanAdquisicionReporte(usuarioId, rubroPresupuestalId, userParams);
                if (pagedList != null)
                {
                    lista = _mapper.Map<List<DetalleCDPDto>>(pagedList);
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaPlanAnualAdquisicion(lista);
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
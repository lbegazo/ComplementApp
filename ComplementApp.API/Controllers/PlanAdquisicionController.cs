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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanAdquisicionController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IGeneralInterface _generalInterface;
        private readonly IPlanAdquisicionRepository _repo;
        private readonly IActividadGeneralRepository _repoActividad;
        private readonly IActividadGeneralService _serviceActividad;
        private readonly IListaRepository _repoLista;
        private readonly IMapper _mapper;
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;

        #endregion Dependency Injection

        public PlanAdquisicionController(IPlanAdquisicionRepository repo,
                                    IActividadGeneralRepository repoActividad,
                                    IActividadGeneralService serviceActividad,
                                    IListaRepository repoLista,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IMapper mapper,
                                    IProcesoCreacionArchivoExcel procesoCreacionExcelInterface)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _repoActividad = repoActividad;
            _serviceActividad = serviceActividad;
            _repoLista = repoLista;
            this._procesoCreacionExcelInterface = procesoCreacionExcelInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerListaPlanAnualAdquisicion()
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                var listaDto = await _repo.ObtenerListaPlanAnualAdquisicion(pciId);
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
                await ActualizarPlanAdquisicion(pciId, planAdquisicion);

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

        private async Task ActualizarPlanAdquisicion(int pciId, PlanAdquisicion planAdquisicion)
        {
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();
            ActividadEspecifica actividadEspecificaBD = null;
            int operacion = 1; // operacion=1=>suma; operacion=2=>resta
            int areaId = 0;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            #region Obtener Area

            if (planAdquisicion.DependenciaId > 0)
            {
                Dependencia dependencia = _dataContext.Dependencia.Where(x => x.DependenciaId == planAdquisicion.DependenciaId).FirstOrDefault();
                if (dependencia != null)
                {
                    areaId = dependencia.AreaId;
                }
            }

            #endregion Obtener Area

            #region Registrar nuevos 

            if (planAdquisicion.EstadoModificacion == (int)EstadoModificacion.Insertado)
            {
                PlanAdquisicion planAdquisicionNuevo = new PlanAdquisicion();
                planAdquisicionNuevo.PlanDeCompras = planAdquisicion.PlanDeCompras;
                planAdquisicionNuevo.ActividadGeneralId = planAdquisicion.ActividadEspecifica.ActividadGeneral.ActividadGeneralId;
                planAdquisicionNuevo.ActividadEspecificaId = planAdquisicion.ActividadEspecifica.ActividadEspecificaId;
                planAdquisicionNuevo.ValorAct = planAdquisicion.ValorAct;
                planAdquisicionNuevo.SaldoAct = planAdquisicion.ValorAct;
                planAdquisicionNuevo.AplicaContrato = planAdquisicion.AplicaContrato;
                planAdquisicionNuevo.UsuarioId = planAdquisicion.UsuarioId;
                planAdquisicionNuevo.DependenciaId = planAdquisicion.DependenciaId;
                planAdquisicionNuevo.AreaId = areaId;
                planAdquisicionNuevo.PciId = pciId;
                planAdquisicionNuevo.EstadoId = (int)EstadoPlanAdquisicion.Generado;
                if (planAdquisicion.RubroPresupuestal != null)
                {
                    planAdquisicionNuevo.RubroPresupuestalId = planAdquisicion.RubroPresupuestal.RubroPresupuestalId;
                    planAdquisicionNuevo.DecretoId = planAdquisicion.RubroPresupuestal.PadreRubroId.Value;
                }

                actividadEspecificaBD = await _repoActividad.ObtenerActividadEspecificaBase(planAdquisicion.ActividadEspecifica.ActividadEspecificaId);

                if (actividadEspecificaBD != null)
                {
                    operacion = 2; // resta
                    await _serviceActividad.ActualizarActividadEspecifica(actividadEspecificaBD, planAdquisicion.ValorAct, operacion);
                }
                await _dataContext.PlanAdquisicion.AddAsync(planAdquisicionNuevo);
                await _dataContext.SaveChangesAsync();
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            if (planAdquisicion.EstadoModificacion == (int)EstadoModificacion.Modificado)
            {
                decimal valor = 0;
                PlanAdquisicion planAdquisicionBD = await _repo.ObtenerPlanAnualAdquisicionBase(planAdquisicion.PlanAdquisicionId);

                if (planAdquisicionBD != null)
                {
                    if (planAdquisicionBD.ValorAct > planAdquisicion.ValorAct)
                    {
                        operacion = 1; // Suma
                        valor = planAdquisicionBD.ValorAct - planAdquisicion.ValorAct;
                    }
                    else
                    {
                        operacion = 2; // Resta
                        valor = planAdquisicion.ValorAct - planAdquisicionBD.ValorAct;
                    }

                    planAdquisicionBD.PlanDeCompras = planAdquisicion.PlanDeCompras;
                    planAdquisicionBD.AplicaContrato = planAdquisicion.AplicaContrato;
                    planAdquisicionBD.SaldoAct = planAdquisicion.ValorAct;
                    planAdquisicionBD.ValorAct = planAdquisicion.ValorAct;
                    planAdquisicionBD.AplicaContrato = planAdquisicion.AplicaContrato;
                    planAdquisicionBD.DependenciaId = planAdquisicion.DependenciaId;
                    planAdquisicionBD.Crp = planAdquisicion.Crp;
                    planAdquisicionBD.AreaId = areaId;
                    await _dataContext.SaveChangesAsync();

                    actividadEspecificaBD = await _repoActividad.ObtenerActividadEspecificaBase(planAdquisicion.ActividadEspecifica.ActividadEspecificaId);

                    if (actividadEspecificaBD != null)
                    {
                        await _serviceActividad.ActualizarActividadEspecifica(actividadEspecificaBD, valor, operacion);
                    }
                }
            }

            #endregion Actualizar registros

            await transaction.CommitAsync();
        }
    }
}
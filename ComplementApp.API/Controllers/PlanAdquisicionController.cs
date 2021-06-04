using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
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

        #endregion Dependency Injection

        public PlanAdquisicionController(IPlanAdquisicionRepository repo,
                                    IActividadGeneralRepository repoActividad,
                                    IActividadGeneralService serviceActividad,
                                    IListaRepository repoLista,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _repoActividad = repoActividad;
            _serviceActividad = serviceActividad;
            _repoLista = repoLista;
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
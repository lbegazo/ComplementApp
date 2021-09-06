using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;

namespace ComplementApp.API.Services
{
    public class PlanAdquisicionService : IPlanAdquisicionService
    {
        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IGeneralInterface _generalInterface;
        private readonly IActividadGeneralRepository _repoActividad;
        private readonly IPlanAdquisicionRepository _repo;
        private readonly IActividadGeneralService _serviceActividad;

        #endregion Dependency Injection

        public PlanAdquisicionService(IPlanAdquisicionRepository repo,
                                   IActividadGeneralRepository repoActividad,
                                   DataContext dataContext,
                                   IGeneralInterface generalInterface,
                                   IMapper mapper,
                                   IActividadGeneralService serviceActividad)
        {
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _repoActividad = repoActividad;
            _serviceActividad = serviceActividad;
        }


        public async Task ActualizarPlanAdquisicion(int pciId, int transaccionId, PlanAdquisicion planAdquisicion)
        {

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
                planAdquisicionNuevo.ValorInicial = planAdquisicion.ValorAct;
                planAdquisicionNuevo.AplicaContrato = planAdquisicion.AplicaContrato;
                planAdquisicionNuevo.UsuarioId = planAdquisicion.UsuarioId;
                planAdquisicionNuevo.DependenciaId = planAdquisicion.DependenciaId;
                planAdquisicionNuevo.AreaId = areaId;
                planAdquisicionNuevo.PciId = pciId;
                planAdquisicionNuevo.EstadoId = (int)EstadoPlanAdquisicion.Generado;
                planAdquisicionNuevo.UsuarioIdRegistro = planAdquisicion.UsuarioIdRegistro;
                planAdquisicionNuevo.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                planAdquisicionNuevo.FechaEstimadaContratacion = planAdquisicion.FechaEstimadaContratacion;

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

                await RegistrarPlanAdquisicionHistorico(planAdquisicionNuevo, planAdquisicion.ValorAct, transaccionId, esDebito: false);
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            if (planAdquisicion.EstadoModificacion == (int)EstadoModificacion.Modificado)
            {
                bool esDebito = false;
                decimal valor = 0;
                PlanAdquisicion planAdquisicionBD = await _repo.ObtenerPlanAnualAdquisicionBase(planAdquisicion.PlanAdquisicionId);

                if (planAdquisicionBD != null)
                {
                    if (planAdquisicion.ValorAct < 0)
                    {
                        operacion = 1; // Suma
                        valor = planAdquisicion.ValorAct;
                    }
                    else
                    {
                        operacion = 2; // Resta
                        valor = planAdquisicion.ValorAct;
                    }

                    planAdquisicionBD.PlanDeCompras = planAdquisicion.PlanDeCompras;
                    planAdquisicionBD.AplicaContrato = planAdquisicion.AplicaContrato;
                    planAdquisicionBD.SaldoAct = planAdquisicionBD.SaldoAct + planAdquisicion.ValorAct;
                    planAdquisicionBD.ValorAct = planAdquisicionBD.ValorAct + planAdquisicion.ValorAct;
                    planAdquisicionBD.ValorModificacion = planAdquisicionBD.ValorModificacion + planAdquisicion.ValorAct;
                    planAdquisicionBD.AplicaContrato = planAdquisicion.AplicaContrato;
                    planAdquisicionBD.DependenciaId = planAdquisicion.DependenciaId;
                    planAdquisicionBD.Crp = planAdquisicion.Crp;
                    planAdquisicionBD.AreaId = areaId;
                    planAdquisicionBD.UsuarioId = planAdquisicion.UsuarioId;
                    planAdquisicionBD.UsuarioIdModificacion = planAdquisicion.UsuarioIdRegistro;
                    planAdquisicionBD.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    planAdquisicionBD.FechaEstimadaContratacion = planAdquisicion.FechaEstimadaContratacion;
                    await _dataContext.SaveChangesAsync();

                    actividadEspecificaBD = await _repoActividad.ObtenerActividadEspecificaBase(planAdquisicion.ActividadEspecifica.ActividadEspecificaId);

                    if (actividadEspecificaBD != null)
                    {
                        await _serviceActividad.ActualizarActividadEspecifica(actividadEspecificaBD, valor, operacion);
                    }

                    //planAdquisicionBD.ValorAct = valor;
                    esDebito = planAdquisicion.ValorAct < 0 ? true : false;
                    await RegistrarPlanAdquisicionHistorico(planAdquisicionBD, valor, transaccionId, esDebito);
                }
            }

            #endregion Actualizar registros

            await transaction.CommitAsync();
        }

        public async Task RegistrarPlanAdquisicionHistorico(PlanAdquisicion planAdquisicion, decimal valor, int transaccionId, bool esDebito)
        {
            PlanAdquisicionHistorico planAdquisicionNuevo = new PlanAdquisicionHistorico();
            planAdquisicionNuevo.PlanAdquisicioId = planAdquisicion.PlanAdquisicionId;
            planAdquisicionNuevo.PlanDeCompras = planAdquisicion.PlanDeCompras;
            planAdquisicionNuevo.ActividadGeneralId = planAdquisicion.ActividadGeneralId;
            planAdquisicionNuevo.ActividadEspecificaId = planAdquisicion.ActividadEspecificaId;

            planAdquisicionNuevo.Valor = valor;
            planAdquisicionNuevo.Saldo = valor;

            planAdquisicionNuevo.AplicaContrato = planAdquisicion.AplicaContrato;
            planAdquisicionNuevo.UsuarioId = planAdquisicion.UsuarioId;
            planAdquisicionNuevo.DependenciaId = planAdquisicion.DependenciaId;
            planAdquisicionNuevo.AreaId = planAdquisicion.AreaId;
            planAdquisicionNuevo.PciId = planAdquisicion.PciId;
            planAdquisicionNuevo.EstadoId = planAdquisicion.EstadoId;
            planAdquisicionNuevo.RubroPresupuestalId = planAdquisicion.RubroPresupuestalId;
            planAdquisicionNuevo.DecretoId = planAdquisicion.DecretoId;

            planAdquisicionNuevo.TransaccionId = transaccionId;
            planAdquisicionNuevo.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
            planAdquisicionNuevo.UsuarioIdRegistro = planAdquisicion.UsuarioIdRegistro;

            await _dataContext.PlanAdquisicionHistorico.AddAsync(planAdquisicionNuevo);
            await _dataContext.SaveChangesAsync();
        }

        public async Task ActualizarPlanAdquisicionExterno(int usuarioId, int transaccionId, int planAdquisicionId,
                                                            string objetoBien, decimal valor, bool esDebito)
        {
            var planAdquisicionBD = await _repo.ObtenerPlanAnualAdquisicionBase(planAdquisicionId);

            if (esDebito)
                planAdquisicionBD.SaldoAct = planAdquisicionBD.SaldoAct - valor;
            else
                planAdquisicionBD.SaldoAct = planAdquisicionBD.SaldoAct + valor;

            planAdquisicionBD.EstadoId = (int)EstadoPlanAdquisicion.ConCDP;
            await _dataContext.SaveChangesAsync();

            await RegistrarPlanAdquisicionHistoricoExterno(planAdquisicionBD, objetoBien, valor, usuarioId, transaccionId, esDebito);
        }
        public async Task RegistrarPlanAdquisicionHistoricoExterno(PlanAdquisicion planAdquisicion, string objetoBien,
                                                                   decimal valor, int usuarioId, int transaccionId, bool esDebito)
        {
            PlanAdquisicionHistorico planAdquisicionNuevo = new PlanAdquisicionHistorico();
            planAdquisicionNuevo.PlanAdquisicioId = planAdquisicion.PlanAdquisicionId;
            planAdquisicionNuevo.PlanDeCompras = objetoBien;
            planAdquisicionNuevo.ActividadGeneralId = planAdquisicion.ActividadGeneralId;
            planAdquisicionNuevo.ActividadEspecificaId = planAdquisicion.ActividadEspecificaId;

            if (esDebito)
                planAdquisicionNuevo.Saldo = (-1) * valor;
            else
                planAdquisicionNuevo.Saldo = valor;

            planAdquisicionNuevo.AplicaContrato = planAdquisicion.AplicaContrato;
            planAdquisicionNuevo.UsuarioId = planAdquisicion.UsuarioId;
            planAdquisicionNuevo.DependenciaId = planAdquisicion.DependenciaId;
            planAdquisicionNuevo.AreaId = planAdquisicion.AreaId;
            planAdquisicionNuevo.PciId = planAdquisicion.PciId;
            planAdquisicionNuevo.EstadoId = planAdquisicion.EstadoId;
            planAdquisicionNuevo.RubroPresupuestalId = planAdquisicion.RubroPresupuestalId;
            planAdquisicionNuevo.DecretoId = planAdquisicion.DecretoId;

            planAdquisicionNuevo.TransaccionId = transaccionId;
            planAdquisicionNuevo.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
            planAdquisicionNuevo.UsuarioIdRegistro = usuarioId;

            await _dataContext.PlanAdquisicionHistorico.AddAsync(planAdquisicionNuevo);
            await _dataContext.SaveChangesAsync();
        }

    }
}
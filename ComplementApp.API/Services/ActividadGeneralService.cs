using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;

namespace ComplementApp.API.Services
{
    public class ActividadGeneralService : IActividadGeneralService
    {
        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IGeneralInterface _generalInterface;
        private readonly IActividadGeneralRepository _repoActividad;
        private readonly IPlanAdquisicionRepository _repo;
        private readonly IMapper _mapper;

        #endregion Dependency Injection

        public ActividadGeneralService(IPlanAdquisicionRepository repo,
                                   IActividadGeneralRepository repoActividad,
                                   DataContext dataContext,
                                   IGeneralInterface generalInterface,
                                   IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _repoActividad = repoActividad;
        }

        public async Task ActualizarListaActividadGeneral(int pciId, List<ActividadGeneral> listaTotal)
        {
            List<ActividadGeneral> listaActividadGeneral = new List<ActividadGeneral>();
            ActividadGeneral actividadGeneral = null;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();

            #region Registrar nuevos 

            List<ActividadGeneral> listaNueva = listaTotal
                                                .Where(x => x.ActividadGeneralId == 0)
                                                .OrderBy(x => x.RubroPresupuestal.Nombre)
                                                .ToList();

            if (listaNueva != null && listaNueva.Count > 0)
            {
                foreach (var item in listaNueva)
                {
                    actividadGeneral = new ActividadGeneral();
                    actividadGeneral.PciId = pciId;
                    actividadGeneral.ApropiacionVigente = item.ApropiacionVigente;
                    actividadGeneral.ApropiacionDisponible = item.ApropiacionDisponible;
                    actividadGeneral.RubroPresupuestalId = item.RubroPresupuestal.RubroPresupuestalId;
                    listaActividadGeneral.Add(actividadGeneral);
                }
                await _dataContext.ActividadGeneral.AddRangeAsync(listaActividadGeneral);
                await _dataContext.SaveChangesAsync();
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            List<ActividadGeneral> listaModificada = listaTotal
                                                    .Where(x => x.ActividadGeneralId > 0)
                                                    .ToList();

            if (listaModificada != null && listaModificada.Count > 0)
            {
                foreach (var item in listaModificada)
                {
                    actividadGeneral = await _repoActividad.ObtenerActividadGeneralBase(item.ActividadGeneralId);

                    if (actividadGeneral != null)
                    {
                        actividadGeneral.ApropiacionVigente = item.ApropiacionVigente;
                        actividadGeneral.ApropiacionDisponible = item.ApropiacionDisponible;
                        await _dataContext.SaveChangesAsync();
                    }
                }
            }

            #endregion Actualizar registros
        }

        public async Task ActualizarActividadEspecifica(int pciId, ActividadEspecifica actividadEspecifica)
        {
            ActividadGeneral actividadGeneralBD = null;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();
            int operacion = 1; // operacion=1=>suma; operacion=2=>resta

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            #region Registrar nuevos 

            if (actividadEspecifica.EstadoModificacion == (int)EstadoModificacion.Insertado)
            {
                ActividadEspecifica actividadEspecificaNuevo = new ActividadEspecifica();
                actividadEspecificaNuevo.Nombre = actividadEspecifica.Nombre;
                actividadEspecificaNuevo.RubroPresupuestalId = actividadEspecifica.RubroPresupuestal.RubroPresupuestalId;
                actividadEspecificaNuevo.SaldoPorProgramar = actividadEspecifica.SaldoPorProgramar;
                actividadEspecificaNuevo.ValorApropiacionVigente = actividadEspecifica.ValorApropiacionVigente;
                actividadEspecificaNuevo.ActividadGeneralId = actividadEspecifica.ActividadGeneral.ActividadGeneralId;
                actividadEspecificaNuevo.PciId = pciId;
                actividadGeneralBD = await _repoActividad.ObtenerActividadGeneralBase(actividadEspecifica.ActividadGeneral.ActividadGeneralId);

                if (actividadGeneralBD != null)
                {
                    operacion = 2; // resta
                    await ActualizarActividadGeneral(actividadGeneralBD, actividadEspecifica.SaldoPorProgramar, operacion);
                }
                await _dataContext.ActividadEspecifica.AddAsync(actividadEspecificaNuevo);
                await _dataContext.SaveChangesAsync();
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            if (actividadEspecifica.EstadoModificacion == (int)EstadoModificacion.Modificado)
            {
                decimal valor = 0;
                ActividadEspecifica actividadEspecificaBD = await _repoActividad.ObtenerActividadEspecificaBase(actividadEspecifica.ActividadEspecificaId);

                if (actividadEspecificaBD != null)
                {
                    if (actividadEspecificaBD.SaldoPorProgramar > actividadEspecifica.SaldoPorProgramar)
                    {
                        operacion = 1; // Suma
                        valor = actividadEspecificaBD.SaldoPorProgramar - actividadEspecifica.SaldoPorProgramar;
                    }
                    else
                    {
                        operacion = 2; // Resta
                        valor = actividadEspecifica.SaldoPorProgramar - actividadEspecificaBD.SaldoPorProgramar;
                    }

                    actividadEspecificaBD.Nombre = actividadEspecifica.Nombre;
                    actividadEspecificaBD.ValorApropiacionVigente = actividadEspecifica.ValorApropiacionVigente;
                    actividadEspecificaBD.SaldoPorProgramar = actividadEspecifica.SaldoPorProgramar;
                    actividadEspecificaBD.PciId = pciId;
                    await _dataContext.SaveChangesAsync();

                    actividadGeneralBD = await _repoActividad.ObtenerActividadGeneralBase(actividadEspecifica.ActividadGeneral.ActividadGeneralId);

                    if (actividadGeneralBD != null)
                    {

                        await ActualizarActividadGeneral(actividadGeneralBD, valor, operacion);
                    }
                }
            }

            #endregion Actualizar registros

            await transaction.CommitAsync();
        }

        public async Task ActualizarActividadGeneral(ActividadGeneral actividadGeneral, decimal valor, int operacion)
        {
            if (operacion == 1)
            {
                actividadGeneral.ApropiacionDisponible = actividadGeneral.ApropiacionDisponible + valor;
            }
            else
            {
                actividadGeneral.ApropiacionDisponible = actividadGeneral.ApropiacionDisponible - valor;
            }

            await _dataContext.SaveChangesAsync();
        }

        public async Task ActualizarActividadEspecifica(ActividadEspecifica actividad, decimal valor, int operacion)
        {
            if (operacion == 1)
            {
                valor = (-1) * valor;
                actividad.SaldoPorProgramar = actividad.SaldoPorProgramar + valor;
            }
            else
            {
                actividad.SaldoPorProgramar = actividad.SaldoPorProgramar - valor;
            }
            await _dataContext.SaveChangesAsync();
        }
    }
}
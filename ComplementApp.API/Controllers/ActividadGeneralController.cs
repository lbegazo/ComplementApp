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
    public class ActividadGeneralController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IGeneralInterface _generalInterface;
        private readonly IActividadGeneralRepository _repo;
        private readonly IMapper _mapper;

        #endregion Dependency Injection

        public ActividadGeneralController(IActividadGeneralRepository repo,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerActividadesGenerales()
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                var listaDto = await _repo.ObtenerActividadesGenerales(pciId);
                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de Rubros Presupuestales a nivel Decreto");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegistrarActividadesGenerales(ActividadPrincipalDto principal)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (principal != null)
            {
                if (principal.ListaActividadGeneral != null && principal.ListaActividadGeneral.Count > 0)
                {
                    await ActualizarListaActividadGeneral(pciId, principal.ListaActividadGeneral.ToList());

                    await transaction.CommitAsync();
                    return Ok(1);
                }
            }
            return NoContent();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerActividadesEspecificas()
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                var listaDto = await _repo.ObtenerActividadesEspecificas(pciId);
                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de actividades específicas");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegistrarActividadesEspecificas(ActividadPrincipalDto principal)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            if (principal != null)
            {
                if (principal.ListaActividadEspecifica != null && principal.ListaActividadEspecifica.Count > 0)
                {
                    await ActualizarListaActividadEspecifica(pciId, principal.ListaActividadEspecifica.ToList());

                    return Ok(1);
                }
            }
            return NoContent();
        }

        private async Task ActualizarListaActividadGeneral(int pciId, List<ActividadGeneral> listaTotal)
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
                    actividadGeneral = await _repo.ObtenerActividadGeneralBase(item.ActividadGeneralId);

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

        private async Task ActualizarListaActividadEspecifica(int pciId, List<ActividadEspecifica> listaTotal)
        {
            List<ActividadEspecifica> listaActividad = new List<ActividadEspecifica>();
            ActividadEspecifica actividadEspecifica = null;
            ActividadGeneral actividadGeneral = null;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            #region Registrar nuevos 

            List<ActividadEspecifica> listaNueva = listaTotal
                                                .Where(x => x.EstadoModificacion == (int)EstadoModificacion.Insertado)
                                                .ToList();

            if (listaNueva != null && listaNueva.Count > 0)
            {
                foreach (var item in listaNueva)
                {
                    actividadEspecifica = new ActividadEspecifica();
                    actividadEspecifica.Nombre = item.Nombre;
                    actividadEspecifica.PciId = pciId;
                    actividadEspecifica.ValorApropiacionVigente = item.ValorApropiacionVigente;
                    actividadEspecifica.SaldoPorProgramar = item.SaldoPorProgramar;
                    actividadEspecifica.RubroPresupuestalId = item.RubroPresupuestal.RubroPresupuestalId;
                    actividadEspecifica.ActividadGeneralId = item.ActividadGeneral.ActividadGeneralId;
                    listaActividad.Add(actividadEspecifica);

                    actividadGeneral = await _repo.ObtenerActividadGeneralBase(item.ActividadGeneral.ActividadGeneralId);

                    if (actividadGeneral != null)
                    {
                        await ActualizarActividadGeneral(actividadGeneral, item.SaldoPorProgramar);
                    }
                }
                await _dataContext.ActividadEspecifica.AddRangeAsync(listaActividad);
                await _dataContext.SaveChangesAsync();
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            List<ActividadEspecifica> listaModificada = listaTotal
                                                    .Where(x => x.ActividadEspecificaId > 0)
                                                    .Where(x => x.EstadoModificacion == (int)EstadoModificacion.Modificado)
                                                    .ToList();

            if (listaModificada != null && listaModificada.Count > 0)
            {
                foreach (var item in listaModificada)
                {
                    actividadEspecifica = await _repo.ObtenerActividadEspecificaBase(item.ActividadEspecificaId);

                    if (actividadEspecifica != null)
                    {
                        actividadEspecifica.Nombre = item.Nombre;
                        actividadEspecifica.ValorApropiacionVigente = item.ValorApropiacionVigente;
                        actividadEspecifica.SaldoPorProgramar = item.SaldoPorProgramar;
                        await _dataContext.SaveChangesAsync();
                    }

                    actividadGeneral = await _repo.ObtenerActividadGeneralBase(item.ActividadGeneral.ActividadGeneralId);

                    if (actividadGeneral != null)
                    {
                        await ActualizarActividadGeneral(actividadGeneral, item.SaldoPorProgramar);
                    }
                }
            }

            #endregion Actualizar registros

            #region Eliminar registros

            List<ActividadEspecifica> listaEliminada = listaTotal
                                                    .Where(x => x.ActividadEspecificaId > 0)
                                                    .Where(x => x.EstadoModificacion == (int)EstadoModificacion.Eliminado)
                                                    .ToList();

            if (listaEliminada != null && listaEliminada.Count > 0)
            {
                _dataContext.ActividadEspecifica.RemoveRange(listaEliminada);
                _dataContext.SaveChanges();
            }

            #endregion Eliminar registros

            await transaction.CommitAsync();
        }

        private async Task ActualizarActividadGeneral(ActividadGeneral actividadGeneral, decimal valor)
        {
            actividadGeneral.ApropiacionDisponible = actividadGeneral.ApropiacionDisponible - valor;
            await _dataContext.SaveChangesAsync();
        }

    }
}
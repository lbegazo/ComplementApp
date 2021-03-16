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
                var listaDto = await _repo.ObtenerActividadesGenerales();
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
        public async Task<IActionResult> RegistrarActividadesGenerales(ActividadGeneralPrincipalDto principal)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (principal != null)
            {
                if (principal.ListaActividadGeneral != null && principal.ListaActividadGeneral.Count > 0)
                {
                    await ActualizarListaActividadGeneral(principal.ListaActividadGeneral.ToList());

                    await transaction.CommitAsync();
                    return Ok(1);
                }
            }
            return NoContent();
        }

        private async Task ActualizarListaActividadGeneral(List<ActividadGeneral> listaTotal)
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



    }
}
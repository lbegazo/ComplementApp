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
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
     public class ActividadGeneralController : BaseApiController
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
        private readonly IActividadGeneralService _serviceActividad;
        private readonly IMapper _mapper;

        #endregion Dependency Injection

        public ActividadGeneralController(IActividadGeneralRepository repo,
        IActividadGeneralService serviceActividad,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _serviceActividad = serviceActividad;
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
                    await _serviceActividad.ActualizarListaActividadGeneral(pciId, principal.ListaActividadGeneral.ToList());

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

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaActividadEspecifica([FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerListaActividadEspecifica(userParams);
            var listaDto = _mapper.Map<IEnumerable<ActividadEspecifica>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegistrarActividadesEspecificas(ActividadEspecifica actividadEspecifica)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            if (actividadEspecifica != null)
            {
                await _serviceActividad.ActualizarActividadEspecifica(pciId, actividadEspecifica);

                return Ok(1);
            }
            return NoContent();
        }

    }
}
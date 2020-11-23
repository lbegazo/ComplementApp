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
    public class CDPController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ICDPRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGeneralInterface _generalInterface;
        public IUsuarioRepository UsuarioRepo => _usuarioRepo;

        #endregion Dependency Injection

        public CDPController(ICDPRepository repo, IUsuarioRepository usuarioRepo, IMapper mapper, DataContext dataContext, IGeneralInterface generalInterface)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCDP()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cdps = await _repo.ObtenerListaCDP(usuarioId);
            return base.Ok(cdps);
        }

        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCDP(int numeroCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cdps = await _repo.ObtenerCDP(usuarioId, numeroCDP);
            return base.Ok(cdps);
        }

        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleDeCDP(int numeroCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var CDPs = await _repo.ObtenerDetalleDeCDP(usuarioId, numeroCDP);
            return Ok(CDPs);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> RegistrarSolicitudCDP(SolicitudCDP solicitudCDP)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (solicitudCDP != null)
                {
                    solicitudCDP.UsuarioIdRegistro = usuarioId;
                    solicitudCDP.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    var tipoOperacion = _dataContext.TipoOperacion
                                            .Where(t => t.TipoOperacionId == solicitudCDP.TipoOperacion.TipoOperacionId)
                                            .FirstOrDefault();

                    var estadoSolicitudCDP = _dataContext.EstadoSolicitudCDP
                    .Where(t => t.EstadoId == solicitudCDP.EstadoSolicitudCDP.EstadoId)
                    .FirstOrDefault();

                    if (estadoSolicitudCDP != null)
                    {
                        solicitudCDP.EstadoSolicitudCDPId = estadoSolicitudCDP.EstadoId;
                        solicitudCDP.EstadoSolicitudCDP = estadoSolicitudCDP;
                    }

                    if (tipoOperacion != null)
                    {
                        solicitudCDP.TipoOperacionId = tipoOperacion.TipoOperacionId;
                        solicitudCDP.TipoOperacion = tipoOperacion;

                        if (tipoOperacion.TipoOperacionId != (int)TipoOperacionEnum.SOLICITUD_INICIAL)
                        {
                            var tipoDetalle = _dataContext.TipoDetalleModificacion
                                                .Where(t => t.TipoDetalleCDPId == solicitudCDP.TipoDetalleCDP.TipoDetalleCDPId)
                                                .FirstOrDefault();

                            if (tipoDetalle != null)
                            {
                                solicitudCDP.TipoDetalleCDPId = tipoDetalle.TipoDetalleCDPId;
                                solicitudCDP.TipoDetalleCDP = tipoDetalle;
                            }
                        }
                        else
                        {
                            solicitudCDP.Cdp = null;
                            solicitudCDP.TipoDetalleCDPId = null;
                        }
                    }

                    if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                    {
                        foreach (var detalle in solicitudCDP.DetalleSolicitudCDPs)
                        {
                            var rubroPresupuestal = _dataContext.RubroPresupuestal
                                                        .Where(r => r.RubroPresupuestalId == detalle.RubroPresupuestal.RubroPresupuestalId)
                                                        .FirstOrDefault();

                            detalle.RubroPresupuestal = rubroPresupuestal;
                            detalle.SolicitudCDP = solicitudCDP;
                        }
                    }

                    //Insertar cabecera
                    await _dataContext.SolicitudCDP.AddAsync(solicitudCDP);
                    await _dataContext.SaveChangesAsync();

                    //Insertar detalle de solicitud de CDP
                    // if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                    // {
                    //     foreach (var detalle in solicitudCDP.DetalleSolicitudCDPs)
                    //     {                            
                    //         await _dataContext.DetalleSolicitudCDP.AddAsync(detalle);
                    //     }
                    // }
                    // await _dataContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(solicitudCDP.SolicitudCDPId);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo registrar la solicitud de CDP");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerSolicitudCDP([FromQuery(Name = "solicitudId")] int solicitudId)
        {
            SolicitudCDPDto solicitudCDP = null;

            try
            {
                if (solicitudId > 0)
                {
                    solicitudCDP = await _repo.ObtenerSolicitudCDP(solicitudId);
                    var lista = await _repo.ObtenerDetalleSolicitudCDP(solicitudId);

                    if (lista != null && lista.Count > 0)
                    {
                        solicitudCDP.DetalleSolicitudCDPs = lista.ToList();
                    }
                    return Ok(solicitudCDP);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la solicitud de CDP");
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerListaSolicitudCDP([FromQuery(Name = "solicitudId")] int? solicitudId,
                                            [FromQuery(Name = "tipoOperacionId")] int? tipoOperacionId,
                                            [FromQuery(Name = "usuarioId")] int? usuarioId,
                                            [FromQuery(Name = "fechaRegistro")] DateTime? fechaRegistro,
                                            [FromQuery(Name = "estadoSolicitudId")] int? estadoSolicitudId,
                                            [FromQuery] UserParams userParams)
        {
            try
            {
                if (fechaRegistro != null)
                {
                    fechaRegistro = fechaRegistro.Value.Date;
                }

                var pagedList = await _repo.ObtenerListaSolicitudCDP(solicitudId, tipoOperacionId, usuarioId,
                                                                    fechaRegistro, estadoSolicitudId, userParams);

                var listaDto = _mapper.Map<IEnumerable<SolicitudCDPParaPrincipalDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

                return base.Ok(listaDto);

            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la solicitud de CDP");
        }

    }
}
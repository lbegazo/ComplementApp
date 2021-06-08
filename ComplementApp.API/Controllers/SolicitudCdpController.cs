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
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCdpController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion 


        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ISolicitudCdpRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGeneralInterface _generalInterface;
        public IUsuarioRepository UsuarioRepo => _usuarioRepo;
        #endregion Dependency Injection


        public SolicitudCdpController(ISolicitudCdpRepository repo, IUsuarioRepository usuarioRepo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
        }


        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCDP(int numeroCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cdps = await _repo.ObtenerCDP(usuarioId, numeroCDP);
            return base.Ok(cdps);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> RegistrarSolicitudCDP(SolicitudCDP solicitudCDP)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            SolicitudCDP solicitud = null;
            DetalleSolicitudCDP detalle = null;
            List<DetalleSolicitudCDP> listaDetalle = new List<DetalleSolicitudCDP>();
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (solicitudCDP != null)
                {
                    #region Registrar Solicitud CDP

                    #region Cabecera Solicitud CDP

                    solicitud = new SolicitudCDP();
                    solicitud.TipoOperacionId = solicitudCDP.TipoOperacion.TipoOperacionId;
                    solicitud.UsuarioId = usuarioId;
                    solicitud.UsuarioIdRegistro = usuarioId;
                    solicitud.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    solicitud.EstadoSolicitudCDPId = solicitudCDP.EstadoSolicitudCDP.EstadoId;

                    solicitud.NumeroActividad = solicitudCDP.NumeroActividad;
                    solicitud.AplicaContrato = solicitudCDP.AplicaContrato;
                    solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                    solicitud.ProyectoInversion = solicitudCDP.ProyectoInversion;
                    solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                    solicitud.ActividadProyectoInversion = solicitudCDP.ActividadProyectoInversion;
                    solicitud.ObjetoBienServicioContratado = solicitudCDP.ObjetoBienServicioContratado;
                    solicitud.Observaciones = solicitudCDP.Observaciones;

                    if (solicitudCDP.TipoOperacion.TipoOperacionId != (int)TipoOperacionEnum.SOLICITUD_INICIAL)
                    {
                        solicitud.TipoDetalleCDPId = solicitudCDP.TipoDetalleCDP.TipoDetalleCDPId;
                    }
                    else
                    {
                        solicitud.Cdp = null;
                        solicitud.TipoDetalleCDPId = null;
                    }

                    //Insertar cabecera
                    await _dataContext.SolicitudCDP.AddAsync(solicitud);
                    await _dataContext.SaveChangesAsync();

                    #endregion Cabecera Solicitud CDP

                    if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                    {
                        #region Registrar Detalle Solicitud CDP

                        foreach (var item in solicitudCDP.DetalleSolicitudCDPs)
                        {
                            detalle = new DetalleSolicitudCDP();
                            detalle.SolicitudCDPId = solicitud.SolicitudCDPId;
                            detalle.PlanAdquisicionId = item.PlanAdquisicionId;
                            detalle.RubroPresupuestalId = item.RubroPresupuestal.RubroPresupuestalId;
                            detalle.ValorActividad = item.ValorActividad;
                            detalle.SaldoActividad = item.SaldoActividad;
                            detalle.ValorSolicitud = item.ValorSolicitud;
                            listaDetalle.Add(detalle);
                        }

                        //Insertar Detalle
                        await _dataContext.DetalleSolicitudCDP.AddRangeAsync(listaDetalle);
                        await _dataContext.SaveChangesAsync();

                        #endregion Registrar Detalle Solicitud CDP
                    }

                    #endregion Registrar Solicitud CDP

                    #region Actualizar Plan de adquisición

                    int EstadoPlanAdquisicion_ConCDP = (int)EstadoPlanAdquisicion.ConCDP;
                    var listaPlaAdquisicionId = solicitudCDP.DetalleSolicitudCDPs.Select(x => x.PlanAdquisicionId).ToHashSet().ToList();

                    var listaPlanAdquisicionBase = from pp in _dataContext.PlanAdquisicion
                                                   where listaPlaAdquisicionId.Contains(pp.PlanAdquisicionId)
                                                   select pp;

                    listaPlanAdquisicionBase.BatchUpdate(new PlanAdquisicion
                    {
                        EstadoId = EstadoPlanAdquisicion_ConCDP
                    });
                    _dataContext.SaveChanges();


                    #endregion Actualizar Plan de adquisición

                    // var tipoOperacion = _dataContext.TipoOperacion
                    //                         .Where(t => t.TipoOperacionId == solicitudCDP.TipoOperacion.TipoOperacionId)
                    //                         .FirstOrDefault();

                    // var estadoSolicitudCDP = _dataContext.EstadoSolicitudCDP
                    // .Where(t => t.EstadoId == solicitudCDP.EstadoSolicitudCDP.EstadoId)
                    // .FirstOrDefault();

                    // if (estadoSolicitudCDP != null)
                    // {
                    //     solicitudCDP.EstadoSolicitudCDPId = estadoSolicitudCDP.EstadoId;
                    //     solicitudCDP.EstadoSolicitudCDP = estadoSolicitudCDP;
                    // }

                    // if (tipoOperacion != null)
                    // {
                    //     solicitudCDP.TipoOperacionId = tipoOperacion.TipoOperacionId;
                    //     solicitudCDP.TipoOperacion = tipoOperacion;

                    //     if (tipoOperacion.TipoOperacionId != (int)TipoOperacionEnum.SOLICITUD_INICIAL)
                    //     {
                    //         var tipoDetalle = _dataContext.TipoDetalleModificacion
                    //                             .Where(t => t.TipoDetalleCDPId == solicitudCDP.TipoDetalleCDP.TipoDetalleCDPId)
                    //                             .FirstOrDefault();

                    //         if (tipoDetalle != null)
                    //         {
                    //             solicitudCDP.TipoDetalleCDPId = tipoDetalle.TipoDetalleCDPId;
                    //             solicitudCDP.TipoDetalleCDP = tipoDetalle;
                    //         }
                    //     }
                    //     else
                    //     {
                    //         solicitudCDP.Cdp = null;
                    //         solicitudCDP.TipoDetalleCDPId = null;
                    //     }
                    // }


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

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCDP()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cdps = await _repo.ObtenerListaCDP(usuarioId);
            return base.Ok(cdps);
        }

    }
}
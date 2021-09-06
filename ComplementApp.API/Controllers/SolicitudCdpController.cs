using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    public class SolicitudCdpController : BaseApiController
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;
        int transaccionId = 21;

        #endregion 


        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ISolicitudCdpRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IGeneralInterface _generalInterface;
        public IUsuarioRepository UsuarioRepo => _usuarioRepo;
        private readonly IPlanAdquisicionService _planAdquisicionService;
        private readonly IPlanAdquisicionRepository _planAdquisicionRepo;


        #endregion Dependency Injection


        public SolicitudCdpController(ISolicitudCdpRepository repo, IUsuarioRepository usuarioRepo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IPlanAdquisicionService planAdquisicionService,
                            IPlanAdquisicionRepository planAdquisicionRepo)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _planAdquisicionService = planAdquisicionService;
            _planAdquisicionRepo = planAdquisicionRepo;
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
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            //PlanAdquisicion planAdquisicion = null;
            List<DetalleSolicitudCDP> listaDetalle = new List<DetalleSolicitudCDP>();
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (solicitudCDP != null)
                {
                    if (solicitudCDP.TipoOperacion.TipoOperacionId == (int)TipoOperacionEnum.SOLICITUD_INICIAL)
                    {
                        #region Cabecera Solicitud CDP

                        solicitud = new SolicitudCDP();
                        solicitud.TipoOperacionId = solicitudCDP.TipoOperacion.TipoOperacionId;
                        solicitud.UsuarioId = usuarioId;
                        solicitud.UsuarioIdRegistro = usuarioId;
                        solicitud.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                        solicitud.EstadoSolicitudCDPId = solicitudCDP.EstadoSolicitudCDP.EstadoId;
                        solicitud.PciId = pciId;

                        solicitud.NumeroActividad = solicitudCDP.NumeroActividad;
                        solicitud.AplicaContrato = solicitudCDP.AplicaContrato;
                        solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                        solicitud.ProyectoInversion = solicitudCDP.ProyectoInversion;
                        solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                        solicitud.ActividadProyectoInversion = solicitudCDP.ActividadProyectoInversion;
                        solicitud.ObjetoBienServicioContratado = solicitudCDP.ObjetoBienServicioContratado;
                        solicitud.Observaciones = solicitudCDP.Observaciones;
                        solicitud.Cdp = null;
                        solicitud.TipoDetalleCDPId = null;

                        await _dataContext.SolicitudCDP.AddAsync(solicitud);
                        await _dataContext.SaveChangesAsync();

                        #endregion Cabecera Solicitud CDP

                        #region Registrar Detalle Solicitud CDP

                        if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                        {
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
                        }

                        #endregion Registrar Detalle Solicitud CDP

                        #region Actualizar Plan de adquisición

                        if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                        {
                            foreach (var item in solicitudCDP.DetalleSolicitudCDPs)
                            {
                                await _planAdquisicionService.ActualizarPlanAdquisicionExterno(usuarioId, transaccionId, item.PlanAdquisicionId.Value,
                                                                                                solicitud.ObjetoBienServicioContratado,
                                                                                                item.ValorSolicitud, esDebito: true);
                            }
                        }

                        #endregion Actualizar Plan de adquisición
                    }
                    else
                    {
                        #region Cabecera Solicitud CDP

                        solicitud = new SolicitudCDP();
                        solicitud.TipoOperacionId = solicitudCDP.TipoOperacion.TipoOperacionId;
                        solicitud.UsuarioId = usuarioId;
                        solicitud.UsuarioIdRegistro = usuarioId;
                        solicitud.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                        solicitud.EstadoSolicitudCDPId = solicitudCDP.EstadoSolicitudCDP.EstadoId;
                        solicitud.PciId = pciId;

                        solicitud.NumeroActividad = solicitudCDP.NumeroActividad;
                        solicitud.AplicaContrato = solicitudCDP.AplicaContrato;
                        solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                        solicitud.ProyectoInversion = solicitudCDP.ProyectoInversion;
                        solicitud.NombreBienServicio = solicitudCDP.NombreBienServicio;
                        solicitud.ActividadProyectoInversion = solicitudCDP.ActividadProyectoInversion;
                        solicitud.ObjetoBienServicioContratado = solicitudCDP.ObjetoBienServicioContratado;
                        solicitud.Observaciones = solicitudCDP.Observaciones;
                        solicitud.Cdp = solicitudCDP.Cdp;
                        solicitud.TipoDetalleCDPId = solicitudCDP.TipoDetalleCDP.TipoDetalleCDPId;

                        await _dataContext.SolicitudCDP.AddAsync(solicitud);
                        await _dataContext.SaveChangesAsync();

                        #endregion Cabecera Solicitud CDP

                        #region Registrar Detalle Solicitud CDP

                        if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                        {
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

                            await _dataContext.DetalleSolicitudCDP.AddRangeAsync(listaDetalle);
                            await _dataContext.SaveChangesAsync();
                        }

                        #endregion Registrar Detalle Solicitud CDP

                        #region Actualizar Plan de adquisición

                        if (solicitudCDP.DetalleSolicitudCDPs != null && solicitudCDP.DetalleSolicitudCDPs.Count > 0)
                        {
                            foreach (var item in solicitudCDP.DetalleSolicitudCDPs)
                            {
                                //planAdquisicion = await _planAdquisicionRepo.ObtenerPlanAnualAdquisicionBase(item.PlanAdquisicionId.Value);

                                switch (solicitudCDP.TipoOperacion.TipoOperacionId)
                                {
                                    case (int)TipoOperacionEnum.ADICION:
                                        {
                                            await _planAdquisicionService.ActualizarPlanAdquisicionExterno(usuarioId, transaccionId, item.PlanAdquisicionId.Value, solicitud.ObjetoBienServicioContratado, item.ValorSolicitud, esDebito: true);
                                            //planAdquisicion.SaldoAct = planAdquisicion.SaldoAct - item.ValorSolicitud;
                                            break;
                                        }
                                    case (int)TipoOperacionEnum.REDUCCION:
                                        {
                                            await _planAdquisicionService.ActualizarPlanAdquisicionExterno(usuarioId, transaccionId, item.PlanAdquisicionId.Value, solicitud.ObjetoBienServicioContratado, item.ValorSolicitud, esDebito: true);
                                            //planAdquisicion.SaldoAct = planAdquisicion.SaldoAct + item.ValorSolicitud;
                                            break;
                                        }
                                    case (int)TipoOperacionEnum.ANULACION:
                                        {
                                            await _planAdquisicionService.ActualizarPlanAdquisicionExterno(usuarioId, transaccionId, item.PlanAdquisicionId.Value, solicitud.ObjetoBienServicioContratado, item.ValorSolicitud, esDebito: true);
                                            //planAdquisicion.SaldoAct = planAdquisicion.SaldoAct + item.ValorSolicitud;
                                            break;
                                        }
                                }

                                //planAdquisicion.EstadoId = (int)EstadoPlanAdquisicion.ConCDP;
                                await _dataContext.SaveChangesAsync();
                            }
                        }

                        #endregion Actualizar Plan de adquisición
                    }

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
        [HttpPut]
        public async Task<IActionResult> ActualizarSolicitudCDP(SolicitudCDPDto solicitudCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                //Registrar Parametro liquidación Tercero
                var solicitudBD = await _repo.ObtenerSolicitudCdpBase(solicitudCDP.SolicitudCDPId);

                #region Mapear datos Tercero

                solicitudBD.Cdp = solicitudCDP.Cdp;
                solicitudBD.UsuarioIdModificacion = usuarioId;
                solicitudBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                #endregion Mapear datos Tercero

                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                    userParams.PciId = pciId;
                }

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
        public async Task<ActionResult> ObtenerListaSolicitudParaVincularCDP([FromQuery(Name = "tipo")] int tipo,
                                            [FromQuery(Name = "numeroSolicitud")] int? numeroSolicitud,
                                            [FromQuery] UserParams userParams)
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                    userParams.PciId = pciId;
                }

                var pagedList = await _repo.ObtenerListaSolicitudParaVincularCDP(tipo, numeroSolicitud, userParams);

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
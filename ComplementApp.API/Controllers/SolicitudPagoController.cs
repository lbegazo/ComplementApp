using System;
using System.Collections.Generic;
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
    public class SolicitudPagoController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ISolicitudPagoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly IListaRepository _listaRepository;
        private readonly IPlanPagoRepository _planPagoRepository;

        private readonly IProcesoLiquidacionSolicitudPago _procesoLiquidacion;

        #endregion Dependency Injection


        public SolicitudPagoController(ISolicitudPagoRepository repo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IPlanPagoRepository planPagoRepository, IListaRepository listaRepository,
                            IProcesoLiquidacionSolicitudPago procesoLiquidacion)
        {
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _listaRepository = listaRepository;
            _planPagoRepository = planPagoRepository;
            _procesoLiquidacion = procesoLiquidacion;
        }

        #region Registro de Solicitud de Pago

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActualizarFormatoSolicitudPago(FormatoSolicitudPagoDto formatoDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DetalleFormatoSolicitudPago detalleSolicitud = null;
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (formatoDto != null)
                {

                    #region Actualizar solicitud de pago
                    var formatoBD = await _repo.ObtenerFormatoSolicitudPagoBase(formatoDto.FormatoSolicitudPagoId);

                    formatoBD.UsuarioIdModificacion = usuarioId;
                    formatoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    formatoBD.ObservacionesModificacion = formatoDto.ObservacionesModificacion;
                    formatoBD.EstadoId = formatoDto.EstadoId;
                    await _dataContext.SaveChangesAsync();

                    #endregion Actualizar solicitud de pago

                    #region Registrar Rubros Presupuestales

                    if (formatoDto.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                    {
                        if (formatoDto.detallesFormatoSolicitudPago != null && formatoDto.detallesFormatoSolicitudPago.Count > 0)
                        {
                            foreach (var item in formatoDto.detallesFormatoSolicitudPago)
                            {
                                if (item.ValorAPagar > 0)
                                {
                                    detalleSolicitud = new DetalleFormatoSolicitudPago();
                                    detalleSolicitud.FormatoSolicitudPagoId = formatoBD.FormatoSolicitudPagoId;
                                    detalleSolicitud.RubroPresupuestalId = item.RubroPresupuestal.Id;
                                    detalleSolicitud.ValorAPagar = item.ValorAPagar;
                                    _dataContext.DetalleFormatoSolicitudPago.Add(detalleSolicitud);
                                }
                            }
                        }
                        await _dataContext.SaveChangesAsync();
                    }

                    #endregion Registrar Rubros Presupuestales                  

                    #region Actualizar el plan de pago

                    if (formatoDto.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                    {
                        var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formatoDto.PlanPagoId);
                        planPagoBD.NumeroFactura = formatoDto.NumeroFactura;
                        planPagoBD.ValorFacturado = formatoDto.ValorFacturado;
                        planPagoBD.Observaciones = formatoDto.Observaciones;
                        planPagoBD.NumeroRadicadoProveedor = formatoDto.NumeroRadicadoProveedor;
                        planPagoBD.FechaRadicadoProveedor = formatoDto.FechaRadicadoProveedor;
                        planPagoBD.NumeroRadicadoSupervisor = formatoDto.NumeroRadicadoSupervisor;
                        planPagoBD.FechaRadicadoSupervisor = formatoDto.FechaRadicadoSupervisor;
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                        planPagoBD.UsuarioIdModificacion = usuarioId;
                        planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        await _dataContext.SaveChangesAsync();
                    }
                    else
                    {
                        var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formatoDto.PlanPagoId);
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorPagar;
                        planPagoBD.UsuarioIdModificacion = usuarioId;
                        planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    }
                    await transaction.CommitAsync();
                    
                    #endregion Actualizar el plan de pago

                    return Ok(formatoBD.FormatoSolicitudPagoId);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarFormatoSolicitudPago(FormatoSolicitudPagoParaGuardarDto formatoDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            FormatoSolicitudPago formato = null;

            try
            {
                if (formatoDto != null)
                {
                    #region Mapear datos 

                    formato = _mapper.Map<FormatoSolicitudPago>(formatoDto);
                    formato.ActividadEconomicaId = formatoDto.ActividadEconomicaId;
                    formato.EstadoId = (int)EstadoSolicitudPago.Generado;
                    formato.UsuarioIdRegistro = usuarioId;
                    formato.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos 

                    //Registrar detalle de liquidación
                    _dataContext.FormatoSolicitudPago.Add(formato);
                    await _dataContext.SaveChangesAsync();

                    #region Actualizar el plan de pago

                    var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formatoDto.PlanPagoId);
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.ConSolicitudPago;
                    planPagoBD.UsuarioIdModificacion = usuarioId;
                    planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.SaveChangesAsync();

                    #endregion Actualizar el plan de pago

                    await transaction.CommitAsync();

                    return Ok(formato.FormatoSolicitudPagoId);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaSolicitudRegistroPago([FromQuery(Name = "usuarioId")] int usuarioId,
                                                                                    [FromQuery(Name = "perfilId")] int perfilId,
                                                                                    [FromQuery(Name = "terceroId")] int? terceroId,
                                                                                    [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerCompromisosParaSolicitudRegistroPago(usuarioId, perfilId, terceroId, userParams);
                var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de compromisos");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerSolicitudesPagoParaAprobar([FromQuery(Name = "usuarioId")] int usuarioId,
                                                                            [FromQuery(Name = "terceroId")] int? terceroId,
                                                                            [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerSolicitudesPagoParaAprobar(usuarioId, terceroId, userParams);
                var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                        pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de Solicitudes de Pago");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoSolicitudPago([FromQuery(Name = "crp")] int crp)
        {
            FormatoSolicitudPagoDto formato = null;

            try
            {
                formato = await _repo.ObtenerFormatoSolicitudPago(crp);
                if (formato != null)
                {
                    var CantidadMaxima = _planPagoRepository.ObtenerCantidadMaximaPlanPago(formato.Cdp.Crp);

                    formato.CantidadMaxima = CantidadMaxima;
                    formato.ValorPagadoFechaActual = formato.Cdp.ValorTotal - formato.Cdp.SaldoActual;

                    var pagosRealizados = await _repo.ObtenerPagosRealizadosXCompromiso(formato.Cdp.Crp);
                    if (pagosRealizados != null)
                    {
                        formato.NumeroPagoFechaActual = pagosRealizados.Count;
                        formato.PagosRealizados = pagosRealizados;
                    }
                }
                return Ok(formato);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de relaciones contables");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoSolicitudPagoXId([FromQuery(Name = "formatoSolicitudPagoId")] int formatoSolicitudPagoId)
        {
            FormatoSolicitudPagoDto formato = null;

            try
            {
                formato = await _repo.ObtenerFormatoSolicitudPagoXId(formatoSolicitudPagoId);
                if (formato != null)
                {
                    var CantidadMaxima = _planPagoRepository.ObtenerCantidadMaximaPlanPago(formato.Cdp.Crp);

                    formato.CantidadMaxima = CantidadMaxima;
                    formato.ValorPagadoFechaActual = formato.Cdp.ValorTotal - formato.Cdp.SaldoActual;

                    FormatoCausacionyLiquidacionPagos formatoCausacion = await _procesoLiquidacion.ObtenerFormatoSolicitudPago(formato.PlanPagoId, formato.BaseCotizacion, formato.ActividadEconomica.Id);
                    formato.AportePension = formatoCausacion.AportePension;
                    formato.AporteSalud = formatoCausacion.AporteSalud;
                    formato.RiesgoLaboral = formatoCausacion.RiesgoLaboral;
                    formato.FondoSolidaridad = formatoCausacion.FondoSolidaridad;

                    var pagosRealizados = await _repo.ObtenerPagosRealizadosXCompromiso(formato.Cdp.Crp);
                    if (pagosRealizados != null)
                    {
                        formato.NumeroPagoFechaActual = pagosRealizados.Count;
                        formato.PagosRealizados = pagosRealizados;
                    }
                }
                return Ok(formato);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de relaciones contables");
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerSeguridadSocialParaSolicitudPago([FromQuery(Name = "planPagoId")] int planPagoId,
                                                                                            [FromQuery(Name = "valorBaseCotizacion")] decimal valorBaseCotizacion,
                                                                                            [FromQuery(Name = "actividadEconomicaId")] int? actividadEconomicaId)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {

                formato = await _procesoLiquidacion.ObtenerFormatoSolicitudPago(planPagoId, valorBaseCotizacion, actividadEconomicaId);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(formato);
        }


        #endregion Registro de Solicitud de Pago


    }
}
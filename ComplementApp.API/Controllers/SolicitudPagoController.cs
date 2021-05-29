using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
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
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ISolicitudPagoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly IListaRepository _listaRepository;
        private readonly IPlanPagoRepository _planPagoRepository;
        private readonly IProcesoLiquidacionSolicitudPago _procesoLiquidacion;
        private readonly ITerceroRepository _terceroRepository;

        #endregion Dependency Injection


        public SolicitudPagoController(ISolicitudPagoRepository repo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IPlanPagoRepository planPagoRepository, IListaRepository listaRepository,
                            IProcesoLiquidacionSolicitudPago procesoLiquidacion,
                            ITerceroRepository terceroRepository)
        {
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _listaRepository = listaRepository;
            _planPagoRepository = planPagoRepository;
            _procesoLiquidacion = procesoLiquidacion;
            _terceroRepository = terceroRepository;
        }

        #region Solicitud de Pago

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActualizarFormatoSolicitudPago(FormatoSolicitudPagoDto formatoDto)
        {

            DetalleFormatoSolicitudPago detalleSolicitud = null;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (formatoDto != null)
                {

                    #region Actualizar solicitud de pago

                    var formatoBD = await _repo.ObtenerFormatoSolicitudPagoBase(formatoDto.FormatoSolicitudPagoId);

                    formatoBD.UsuarioIdModificacion = usuarioId;
                    formatoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    formatoBD.ObservacionesModificacion = formatoDto.ObservacionesModificacion;
                    formatoBD.EstadoId = formatoDto.EstadoId;

                    if (formatoDto.EstadoId == (int)EstadoSolicitudPago.Rechazado)
                    {
                        var parametroLiquidacionTercero = await _terceroRepository.ObtenerParametrizacionLiquidacionXTercero(formatoDto.Tercero.TerceroId, pciId);

                        if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                        {
                            formatoBD.NumeroFactura = string.Empty;
                        }
                    }

                    await _dataContext.SaveChangesAsync();

                    #endregion Actualizar solicitud de pago

                    #region Registrar Rubros Presupuestales

                    if (formatoDto.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                    {
                        if (formatoDto.DetallesFormatoSolicitudPago != null && formatoDto.DetallesFormatoSolicitudPago.Count > 0)
                        {
                            foreach (var item in formatoDto.DetallesFormatoSolicitudPago)
                            {
                                if (item.ValorAPagar > 0)
                                {
                                    detalleSolicitud = new DetalleFormatoSolicitudPago();
                                    detalleSolicitud.FormatoSolicitudPagoId = formatoBD.FormatoSolicitudPagoId;
                                    detalleSolicitud.RubroPresupuestalId = item.RubroPresupuestal.Id;
                                    detalleSolicitud.ValorAPagar = item.ValorAPagar;
                                    detalleSolicitud.Dependencia = item.Dependencia;
                                    _dataContext.DetalleFormatoSolicitudPago.Add(detalleSolicitud);
                                }
                            }
                        }
                        await _dataContext.SaveChangesAsync();
                    }

                    #endregion Registrar Rubros Presupuestales                  

                    #region Actualizar el plan de pago

                    var cantidadPlanPagoxCompromiso = await _planPagoRepository.CantidadPlanPagoParaCompromiso(formatoBD.Crp, pciId);

                    if (cantidadPlanPagoxCompromiso > 1)
                    {
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
                    }

                    #endregion Actualizar el plan de pago

                    #region Actualizar Numeración

                    if (formatoDto.EstadoId == (int)EstadoSolicitudPago.Rechazado)
                    {
                        var parametroLiquidacionTercero = await _terceroRepository.ObtenerParametrizacionLiquidacionXTercero(formatoDto.Tercero.TerceroId, pciId);

                        if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                        {
                            var numeracion = await _repo.ObtenerNumeracionxNumeroFactura(formatoDto.NumeroFactura);
                            if (numeracion != null)
                            {
                                numeracion.Utilizado = false;
                                numeracion.FormatoSolicitudPagoId = null;
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                    }

                    #endregion Actualizar Numeración

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
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            FormatoSolicitudPago formato = null;
            RespuestaSolicitudPago respuestaSolicitud = new RespuestaSolicitudPago();
            Numeracion numeracionDisponible = null;

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (formatoDto != null)
                {
                    #region Mapear datos 

                    formato = _mapper.Map<FormatoSolicitudPago>(formatoDto);
                    formato.ActividadEconomicaId = formatoDto.ActividadEconomicaId;
                    formato.EstadoId = (int)EstadoSolicitudPago.Generado;
                    formato.UsuarioIdRegistro = usuarioId;
                    formato.PciId = pciId;
                    formato.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos 

                    #region Numeracion Disponible

                    var parametroLiquidacionTercero = await _terceroRepository.ObtenerParametrizacionLiquidacionXTercero(formatoDto.TerceroId, pciId);

                    if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                    {
                        numeracionDisponible = await _repo.ObtenerUltimaNumeracionDisponible(pciId);
                        if (numeracionDisponible != null)
                        {
                            formato.NumeroFactura = numeracionDisponible.NumeroConsecutivo;
                        }
                    }

                    #endregion Numeracion Disponible

                    _dataContext.FormatoSolicitudPago.Add(formato);
                    await _dataContext.SaveChangesAsync();

                    #region Actualizar el plan de pago

                    var cantidadPlanPagoxCompromiso = await _planPagoRepository.CantidadPlanPagoParaCompromiso(formatoDto.Crp, pciId);

                    if (cantidadPlanPagoxCompromiso > 1)
                    {
                        var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formatoDto.PlanPagoId);
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.ConSolicitudPago;
                        planPagoBD.UsuarioIdModificacion = usuarioId;
                        planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        await _dataContext.SaveChangesAsync();
                    }

                    #endregion Actualizar el plan de pago

                    #region Actualizar Numeracion Utilizada

                    if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                    {
                        if (numeracionDisponible != null)
                        {
                            var numeracionBase = await _repo.ObtenerNumeracionBase(numeracionDisponible.NumeracionId);
                            if (numeracionBase != null)
                            {
                                numeracionBase.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
                                numeracionBase.Utilizado = true;
                                numeracionBase.PciId = pciId;
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                    }

                    #endregion Actualizar Numeracion Utilizada

                    await transaction.CommitAsync();

                    respuestaSolicitud.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
                    respuestaSolicitud.NumeroFactura = formato.NumeroFactura;

                    return Ok(respuestaSolicitud);
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                userParams.PciId = pciId;

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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                userParams.PciId = pciId;
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
        public async Task<ActionResult> ObtenerListaSolicitudPagoAprobada([FromQuery(Name = "usuarioId")] int usuarioId,
                                                                            [FromQuery(Name = "terceroId")] int? terceroId,
                                                                            [FromQuery] UserParams userParams)
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                userParams.PciId = pciId;
                var pagedList = await _repo.ObtenerListaSolicitudPagoAprobada(terceroId, userParams);
                var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                        pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de Solicitudes de Pago Aprobadas");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoSolicitudPago([FromQuery(Name = "crp")] int crp)
        {
            FormatoSolicitudPagoDto formato = null;

            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                formato = await _repo.ObtenerFormatoSolicitudPago(crp, pciId);
                if (formato != null)
                {
                    var CantidadMaxima = _planPagoRepository.ObtenerCantidadMaximaPlanPago(formato.Cdp.Crp, pciId);

                    formato.CantidadMaxima = CantidadMaxima;
                    formato.ValorPagadoFechaActual = formato.Cdp.ValorTotal - formato.Cdp.SaldoActual;

                    var pagosRealizados = await _repo.ObtenerPagosRealizadosXCompromiso(formato.Cdp.Crp, pciId);
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

            throw new Exception($"No se pudo obtener información de la solicitud de pago");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoSolicitudPagoXId([FromQuery(Name = "formatoSolicitudPagoId")] int formatoSolicitudPagoId)
        {
            FormatoSolicitudPagoDto formato = null;

            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                formato = await _repo.ObtenerFormatoSolicitudPagoXId(formatoSolicitudPagoId);
                if (formato != null)
                {
                    var CantidadMaxima = _planPagoRepository.ObtenerCantidadMaximaPlanPago(formato.Cdp.Crp, pciId);

                    formato.CantidadMaxima = CantidadMaxima;
                    formato.ValorPagadoFechaActual = formato.Cdp.ValorTotal - formato.Cdp.SaldoActual;

                    FormatoCausacionyLiquidacionPagos formatoCausacion = await _procesoLiquidacion.ObtenerFormatoSolicitudPago(formato.PlanPagoId, pciId, formato.BaseCotizacion, formato.ActividadEconomica.Id);
                    formato.AportePension = formatoCausacion.AportePension;
                    formato.AporteSalud = formatoCausacion.AporteSalud;
                    formato.RiesgoLaboral = formatoCausacion.RiesgoLaboral;
                    formato.FondoSolidaridad = formatoCausacion.FondoSolidaridad;

                    var pagosRealizados = await _repo.ObtenerPagosRealizadosXCompromiso(formato.Cdp.Crp, pciId);
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                formato = await _procesoLiquidacion.ObtenerFormatoSolicitudPago(planPagoId, pciId, valorBaseCotizacion, actividadEconomicaId);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(formato);
        }

        #endregion Solicitud de Pago

        #region Registrar y Aprobar Solicitud de Pago

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistraryAprobarSolicitudPago(FormatoSolicitudPagoParaGuardarDto formatoDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            FormatoSolicitudPago formato = null;
            RespuestaSolicitudPago respuestaSolicitud = new RespuestaSolicitudPago();
            Numeracion numeracionDisponible = null;
            DetalleFormatoSolicitudPago detalleSolicitud = null;

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (formatoDto != null)
                {
                    #region Mapear datos 

                    formato = new FormatoSolicitudPago();
                    formato.TerceroId = formatoDto.TerceroId;
                    formato.PlanPagoId = formatoDto.PlanPagoId;
                    formato.Crp = formatoDto.Crp;
                    formato.NumeroFactura = formatoDto.NumeroFactura;
                    formato.ValorFacturado = formatoDto.valorFacturado;
                    formato.ActividadEconomicaId = formatoDto.ActividadEconomicaId;
                    formato.FechaInicio = formatoDto.FechaInicio;
                    formato.FechaFinal = formatoDto.FechaFinal;
                    formato.Observaciones = formatoDto.Observaciones;
                    formato.ValorBaseGravableRenta = formatoDto.ValorBaseGravableRenta;
                    formato.ValorIva = formatoDto.ValorIva;
                    formato.NumeroPlanilla = formatoDto.NumeroPlanilla;
                    formato.MesId = formatoDto.MesId;
                    formato.BaseCotizacion = formatoDto.BaseCotizacion;
                    formato.SupervisorId = formatoDto.SupervisorId;
                    formato.ObservacionesModificacion = formatoDto.Observaciones;
                    formato.PciId = pciId;
                    formato.EstadoId = (int)EstadoSolicitudPago.Aprobado;

                    formato.UsuarioIdRegistro = usuarioId;
                    formato.UsuarioIdModificacion = usuarioId;
                    formato.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    formato.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos 

                    #region Numeracion Disponible

                    var parametroLiquidacionTercero = await _terceroRepository.ObtenerParametrizacionLiquidacionXTercero(formatoDto.TerceroId, pciId);

                    if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                    {
                        numeracionDisponible = await _repo.ObtenerUltimaNumeracionDisponible(pciId);
                        if (numeracionDisponible != null)
                        {
                            formato.NumeroFactura = numeracionDisponible.NumeroConsecutivo;
                        }
                    }

                    #endregion Numeracion Disponible

                    _dataContext.FormatoSolicitudPago.Add(formato);
                    await _dataContext.SaveChangesAsync();

                    #region Registrar Rubros Presupuestales

                    if (formato.EstadoId == (int)EstadoSolicitudPago.Aprobado)
                    {
                        if (formatoDto.DetallesFormatoSolicitudPago != null &&
                            formatoDto.DetallesFormatoSolicitudPago.Count > 0)
                        {
                            foreach (var item in formatoDto.DetallesFormatoSolicitudPago)
                            {
                                if (item.ValorAPagar > 0)
                                {
                                    detalleSolicitud = new DetalleFormatoSolicitudPago();
                                    detalleSolicitud.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
                                    detalleSolicitud.RubroPresupuestalId = item.RubroPresupuestal.Id;
                                    detalleSolicitud.ValorAPagar = item.ValorAPagar;
                                    detalleSolicitud.Dependencia = item.Dependencia;
                                    _dataContext.DetalleFormatoSolicitudPago.Add(detalleSolicitud);
                                }
                            }
                        }
                        await _dataContext.SaveChangesAsync();
                    }

                    #endregion Registrar Rubros Presupuestales                  

                    #region Actualizar el plan de pago

                    var cantidadPlanPagoxCompromiso = await _planPagoRepository.CantidadPlanPagoParaCompromiso(formatoDto.Crp, pciId);


                    var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formatoDto.PlanPagoId);
                    if (cantidadPlanPagoxCompromiso > 1)
                    {
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    }
                    planPagoBD.NumeroRadicadoProveedor = formato.FormatoSolicitudPagoId.ToString();
                    planPagoBD.FechaRadicadoProveedor = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.NumeroRadicadoSupervisor = formato.FormatoSolicitudPagoId.ToString();
                    planPagoBD.FechaRadicadoSupervisor = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.ValorFacturado = formatoDto.valorFacturado;
                    planPagoBD.UsuarioIdModificacion = usuarioId;
                    planPagoBD.SaldoDisponible = planPagoBD.SaldoDisponible.Value - formatoDto.valorFacturado;
                    planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.SaveChangesAsync();

                    #endregion Actualizar el plan de pago

                    #region Actualizar Numeracion Utilizada

                    if (parametroLiquidacionTercero != null && parametroLiquidacionTercero.FacturaElectronicaId == 0)
                    {
                        if (numeracionDisponible != null)
                        {
                            var numeracionBase = await _repo.ObtenerNumeracionBase(numeracionDisponible.NumeracionId);
                            if (numeracionBase != null)
                            {
                                numeracionBase.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
                                numeracionBase.Utilizado = true;
                                numeracionBase.PciId = pciId;
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                    }

                    #endregion Actualizar Numeracion Utilizada

                    await transaction.CommitAsync();

                    respuestaSolicitud.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
                    respuestaSolicitud.NumeroFactura = formato.NumeroFactura;

                    return Ok(respuestaSolicitud);
                }
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        #endregion Registrar y Aprobar Solicitud de Pago

    }
}
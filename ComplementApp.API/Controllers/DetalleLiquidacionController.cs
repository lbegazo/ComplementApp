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
    public class DetalleLiquidacionController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion 

        #region Dependency Injection
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDetalleLiquidacionRepository _repo;

        private readonly IPlanPagoRepository _planPagoRepository;
        private readonly IMapper _mapper;
        private readonly IProcesoLiquidacionPlanPago _procesoLiquidacion;
        private readonly IMailService mailService;
        private readonly IGeneralInterface _generalInterface;

        #endregion Dependency Injection

        public DetalleLiquidacionController(IUnitOfWork unitOfWork, IDetalleLiquidacionRepository repo,
                                     IPlanPagoRepository planPagoRepository,
                                    IMapper mapper, DataContext dataContext,
                                    IMailService mailService,
                                    IProcesoLiquidacionPlanPago procesoLiquidacion,
                                    IGeneralInterface generalInterface)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._planPagoRepository = planPagoRepository;
            this._unitOfWork = unitOfWork;
            this.mailService = mailService;
            _dataContext = dataContext;
            this._procesoLiquidacion = procesoLiquidacion;
            this._generalInterface = generalInterface;
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaDetalleLiquidacion([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery] UserParams userParams)
        {
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();

            var pagedList = await _repo.ObtenerListaDetalleLiquidacion(terceroId, listIds, userParams);
            var listaDto = _mapper.Map<IEnumerable<FormatoCausacionyLiquidacionPagos>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoCausacionyLiquidacionPago([FromQuery(Name = "planPagoId")] int planPagoId, [FromQuery(Name = "valorBaseGravable")] decimal valorBaseGravable)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {

                formato = await _procesoLiquidacion.ObtenerFormatoCausacionyLiquidacionPago(planPagoId, valorBaseGravable);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(formato);
        }

        [Route("[action]/{detalleLiquidacionId}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleFormatoCausacionyLiquidacionPago(int detalleLiquidacionId)
        {
            try
            {
                if (detalleLiquidacionId > 0)
                {
                    var formato = await _repo.ObtenerDetalleFormatoCausacionyLiquidacionPago(detalleLiquidacionId);
                    return Ok(formato);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener el formato de liquidación");
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarDetalleLiquidacion(FormatoCausacionyLiquidacionPagos formato)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DetalleLiquidacion detalleLiquidacion = new DetalleLiquidacion();
            LiquidacionDeduccion liquidacionDeduccion = null;
            DetalleLiquidacion detalleLiquidacionAnterior = null;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (formato != null)
                {
                    var detallePlanPago = await _planPagoRepository.ObtenerDetallePlanPago(formato.PlanPagoId);

                    #region Mapear datos 

                    MapearFormatoLiquidacionPlanPago(detallePlanPago, detalleLiquidacion);

                    MapearFormatoLiquidacion(formato, detalleLiquidacion);

                    detalleLiquidacion.UsuarioIdRegistro = usuarioId;
                    detalleLiquidacion.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    detalleLiquidacion.BaseImpuestos = false;

                    #endregion Mapear datos 

                    //Registrar detalle de liquidación
                    _dataContext.DetalleLiquidacion.Add(detalleLiquidacion);
                    await _dataContext.SaveChangesAsync();

                    //Registrar deducciones a la liquidación
                    if (formato.Deducciones != null && formato.Deducciones.Count > 0)
                    {
                        foreach (var deduccion in formato.Deducciones)
                        {
                            liquidacionDeduccion = new LiquidacionDeduccion();
                            liquidacionDeduccion.DetalleLiquidacion = detalleLiquidacion;
                            liquidacionDeduccion.DeduccionId = deduccion.DeduccionId;
                            liquidacionDeduccion.Codigo = deduccion.Codigo;
                            liquidacionDeduccion.Nombre = deduccion.Nombre;
                            liquidacionDeduccion.Tarifa = deduccion.Tarifa;
                            liquidacionDeduccion.Base = deduccion.Base;
                            liquidacionDeduccion.Valor = deduccion.Valor;
                            _dataContext.LiquidacionDeducciones.Add(liquidacionDeduccion);
                        }
                    }
                    await _dataContext.SaveChangesAsync();

                    //Actualizar el estado al plan de pago
                    var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formato.PlanPagoId);
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.ConLiquidacionDeducciones;
                    planPagoBD.UsuarioIdModificacion = usuarioId;
                    planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.SaveChangesAsync();

                    //Actualizar lista de liquidaciones anteriores(Viaticos pagados)
                    var listaDetalleLiquidacionAnterior = await _repo.ObtenerListaDetalleLiquidacionViaticosAnterior(detallePlanPago.TerceroId);

                    if (listaDetalleLiquidacionAnterior != null && listaDetalleLiquidacionAnterior.Count > 0)
                    {
                        foreach (var item in listaDetalleLiquidacionAnterior)
                        {
                            if (item.BaseImpuestos.HasValue && !item.BaseImpuestos.Value)
                            {
                                detalleLiquidacionAnterior = await _repo.ObtenerDetalleLiquidacionBase(item.DetalleLiquidacionId);
                                if (detalleLiquidacionAnterior != null)
                                {
                                    detalleLiquidacionAnterior.BaseImpuestos = true;
                                    detalleLiquidacionAnterior.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                                    detalleLiquidacionAnterior.UsuarioIdModificacion = usuarioId;
                                }
                            }
                        }
                    }
                    await _dataContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(detalleLiquidacion.DetalleLiquidacionId);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        [Route("[action]/{planPagoId}/{mensajeRechazo}")]
        [HttpGet]
        public async Task<IActionResult> RechazarDetalleLiquidacion(int planPagoId, string mensajeRechazo)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            PlanPago planNuevo = new PlanPago();

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (planPagoId > 0)
                {
                    var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(planPagoId);

                    //Actualizar plan de pago existente a estado Por Pagar
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorPagar;
                    planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.UsuarioIdModificacion = usuarioId;
                    await _dataContext.SaveChangesAsync();
                    //_unitOfWork.PlanPagoRepository.ActualizarPlanPago(planPagoBD);
                    //await _unitOfWork.CompleteAsync();

                    //Crear nuevo plan de pago en estado rechazado
                    planNuevo = planPagoBD;
                    planNuevo.PlanPagoId = 0;
                    planNuevo.EstadoPlanPagoId = (int)EstadoPlanPago.Rechazada;
                    planNuevo.MotivoRechazo = mensajeRechazo;
                    planNuevo.UsuarioIdRegistro = usuarioId;
                    planNuevo.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.PlanPago.AddAsync(planNuevo);
                    await _dataContext.SaveChangesAsync();
                    //await _unitOfWork.PlanPagoRepository.RegistrarPlanPago(planNuevo);                    

                    //Enviar email
                    var planPagoDto = await _planPagoRepository.ObtenerDetallePlanPago(planPagoId);
                    await EnviarEmail(planPagoDto, mensajeRechazo);

                    await transaction.CommitAsync();

                    //await _unitOfWork.CompleteAsync();

                    return Ok(planNuevo.PlanPagoId);
                }
            }
            catch (Exception)
            {
                //await _unitOfWork.Rollback();
                //await _unitOfWork.RollbackAsync();
                throw;
            }

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        #region Funciones Generales

        private async Task EnviarEmail(DetallePlanPagoDto planPagoDto, string mensaje)
        {
            try
            {
                if (!string.IsNullOrEmpty(planPagoDto.Email))
                {
                    MailRequest request = new MailRequest();
                    request.ToEmail = planPagoDto.Email;
                    request.Subject = "Radicado de Pago " + planPagoDto.NumeroRadicadoProveedor + " Rechazado";
                    request.Body = "El radicado Nro: " + planPagoDto.NumeroRadicadoProveedor
                                    + " de Fecha " + planPagoDto.FechaRadicadoProveedor.ToString("dd/MM/yyyy")
                                    + " del tercero identificado " + planPagoDto.IdentificacionTercero
                                    + "-" + planPagoDto.NombreTercero + " fue rechazado, motivo: "
                                    + mensaje + "." + "<br>"
                                    + " La línea del plan de pago del compromiso queda en estado"
                                    + " disponible para ser tramitado nuevamente";
                    await mailService.SendEmailAsync(request);
                }
                else
                {
                    throw new Exception("El usuario " + planPagoDto.Usuario + " no tiene registrado el email");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MapearFormatoLiquidacionPlanPago(DetallePlanPagoDto detallePlanPago, DetalleLiquidacion detalleLiquidacion)
        {
            detalleLiquidacion.NumeroIdentificacion = detallePlanPago.IdentificacionTercero;
            detalleLiquidacion.TerceroId = detallePlanPago.TerceroId;
            detalleLiquidacion.Nombre = detallePlanPago.NombreTercero;
            detalleLiquidacion.Contrato = detallePlanPago.Detalle6;
            detalleLiquidacion.Viaticos = detallePlanPago.ViaticosDescripcion;
            detalleLiquidacion.Crp = detallePlanPago.Crp;
            detalleLiquidacion.CantidadPago = detallePlanPago.CantidadPago;
            detalleLiquidacion.NumeroPago = detallePlanPago.NumeroPago;

            detalleLiquidacion.ValorContrato = detallePlanPago.ValorTotal;
            detalleLiquidacion.ValorAdicionReduccion = detallePlanPago.Operacion;
            detalleLiquidacion.ValorCancelado = detallePlanPago.SaldoActual;
            detalleLiquidacion.TotalACancelar = detallePlanPago.ValorFacturado.Value;
            detalleLiquidacion.SaldoActual = (detallePlanPago.SaldoActual - detallePlanPago.ValorFacturado.Value);
            detalleLiquidacion.RubroPresupuestal = detallePlanPago.IdentificacionRubroPresupuestal.ToString();
            detalleLiquidacion.UsoPresupuestal = detallePlanPago.IdentificacionUsoPresupuestal != null ? detallePlanPago.IdentificacionUsoPresupuestal : string.Empty;

            detalleLiquidacion.NombreSupervisor = detallePlanPago.Detalle5;
            detalleLiquidacion.NumeroRadicado = detallePlanPago.NumeroRadicadoSupervisor;
            detalleLiquidacion.FechaRadicado = detallePlanPago.FechaRadicadoSupervisor.Value;
            detalleLiquidacion.NumeroFactura = detallePlanPago.NumeroFactura;
        }

        private void MapearFormatoLiquidacion(FormatoCausacionyLiquidacionPagos formato, DetalleLiquidacion detalleLiquidacion)
        {
            detalleLiquidacion.PlanPagoId = formato.PlanPagoId;
            detalleLiquidacion.CantidadPago = formato.CantidadPago;
            detalleLiquidacion.TextoComprobanteContable = formato.TextoComprobanteContable;

            detalleLiquidacion.ViaticosPagados = formato.ViaticosPagados;
            detalleLiquidacion.Honorario = formato.Honorario;
            detalleLiquidacion.HonorarioUvt = formato.HonorarioUvt;
            detalleLiquidacion.ValorIva = formato.ValorIva;
            detalleLiquidacion.ValorTotal = formato.ValorTotal;
            detalleLiquidacion.TotalRetenciones = formato.TotalRetenciones;
            detalleLiquidacion.TotalAGirar = formato.TotalAGirar;

            detalleLiquidacion.BaseSalud = formato.BaseSalud;
            detalleLiquidacion.AporteSalud = formato.AporteSalud;
            detalleLiquidacion.AportePension = formato.AportePension;
            detalleLiquidacion.RiesgoLaboral = formato.RiesgoLaboral;
            detalleLiquidacion.FondoSolidaridad = formato.FondoSolidaridad;
            detalleLiquidacion.ImpuestoCovid = formato.ImpuestoCovid;
            detalleLiquidacion.SubTotal1 = formato.SubTotal1;

            detalleLiquidacion.PensionVoluntaria = formato.PensionVoluntaria;
            detalleLiquidacion.Afc = formato.Afc;
            detalleLiquidacion.SubTotal2 = formato.SubTotal2;
            detalleLiquidacion.MedicinaPrepagada = formato.MedicinaPrepagada;
            detalleLiquidacion.Dependientes = formato.Dependientes;
            detalleLiquidacion.InteresesVivienda = formato.InteresVivienda;
            detalleLiquidacion.TotalDeducciones = formato.TotalDeducciones;

            detalleLiquidacion.SubTotal3 = formato.SubTotal3;
            detalleLiquidacion.RentaExenta = formato.RentaExenta;
            detalleLiquidacion.LimiteRentaExenta = formato.LimiteRentaExenta;
            detalleLiquidacion.TotalRentaExenta = formato.TotalRentaExenta;
            detalleLiquidacion.DiferencialRenta = formato.DiferencialRenta;
            detalleLiquidacion.BaseGravableRenta = formato.BaseGravableRenta;
            detalleLiquidacion.BaseGravableUvt = formato.BaseGravableUvt;

            detalleLiquidacion.ModalidadContrato = formato.ModalidadContrato;
            detalleLiquidacion.MesSaludAnterior = formato.NumeroMesSaludAnterior;
            detalleLiquidacion.MesSaludActual = formato.NumeroMesSaludActual;
        }

        #endregion Funciones Generales


    }


}
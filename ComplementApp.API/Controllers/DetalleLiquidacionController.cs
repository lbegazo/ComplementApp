using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    public class DetalleLiquidacionController : BaseApiController
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

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
        private readonly IListaRepository _repoLista;
        private readonly ITerceroRepository _terceroRepository;
        private readonly IProcesoCreacionArchivo _procesoCreacionArchivo;
        private readonly ISolicitudPagoRepository _solicitudPagoRepository;

        #endregion Dependency Injection

        public DetalleLiquidacionController(IUnitOfWork unitOfWork, IDetalleLiquidacionRepository repo,
                                     IPlanPagoRepository planPagoRepository,
                                    IMapper mapper, DataContext dataContext,
                                    IMailService mailService,
                                    IProcesoLiquidacionPlanPago procesoLiquidacion,
                                    IGeneralInterface generalInterface,
                                    IListaRepository listaRepository,
                                    ITerceroRepository terceroRepository,
                                    IProcesoCreacionArchivo procesoCreacionArchivo,
                                    ISolicitudPagoRepository solicitudPagoRepository)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._planPagoRepository = planPagoRepository;
            this._unitOfWork = unitOfWork;
            this.mailService = mailService;
            _dataContext = dataContext;
            this._procesoLiquidacion = procesoLiquidacion;
            this._generalInterface = generalInterface;
            this._terceroRepository = terceroRepository;
            this._repoLista = listaRepository;
            this._procesoCreacionArchivo = procesoCreacionArchivo;
            this._solicitudPagoRepository = solicitudPagoRepository;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaDetalleLiquidacionTotal([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery(Name = "procesado")] int? procesado)
        {
            List<int> lista = null;
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
                bool? esProcesado = null;

                if (procesado.HasValue)
                {
                    esProcesado = (procesado.Value == 1) ? (true) : (false);
                }

                lista = await _repo.ObtenerListaDetalleLiquidacionTotal(pciId, terceroId, listIds, esProcesado);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerLiquidacionesParaCuentaPorPagarArchivo([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery(Name = "procesado")] int? procesado,
                                                              [FromQuery] UserParams userParams)
        {
            bool? esProcesado = null;
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            if (procesado.HasValue)
            {
                esProcesado = (procesado.Value == 1) ? (true) : (false);
            }

            var pagedList = await _repo.ObtenerLiquidacionesParaCuentaPorPagarArchivo(terceroId, listIds, esProcesado, userParams);
            var listaDto = _mapper.Map<IEnumerable<FormatoCausacionyLiquidacionPagos>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoCausacionyLiquidacionPago([FromQuery(Name = "solicitudPagoId")] int solicitudPagoId,
                                                                                    [FromQuery(Name = "planPagoId")] int planPagoId,
                                                                                    [FromQuery(Name = "valorBaseGravable")] decimal valorBaseGravable,
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
                formato = await _procesoLiquidacion.ObtenerFormatoCausacionyLiquidacionPago(solicitudPagoId, planPagoId, pciId, valorBaseGravable, actividadEconomicaId);
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                if (formato != null)
                {
                    var detallePlanPago = await _planPagoRepository.ObtenerDetallePlanPago(formato.PlanPagoId);

                    #region Mapear datos 

                    MapearFormatoLiquidacionPlanPago(detallePlanPago, detalleLiquidacion);

                    MapearFormatoLiquidacion(formato, detalleLiquidacion);

                    detalleLiquidacion.UsuarioIdRegistro = usuarioId;
                    detalleLiquidacion.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    detalleLiquidacion.BaseImpuestos = false;
                    detalleLiquidacion.PciId = pciId;
                    detalleLiquidacion.EstadoId = (int)EstadoDetalleLiquidacion.Generado;

                    #endregion Mapear datos 

                    #region Registrar Detalle de Liquidacion

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

                    #endregion Registrar Detalle de Liquidacion

                    #region Actualizar el estado al plan de pago

                    var cantidadPlanPagoxCompromiso = await _planPagoRepository.CantidadPlanPagoParaCompromiso(detallePlanPago.Crp, pciId);

                    if (cantidadPlanPagoxCompromiso > 1)
                    {
                        var planPagoBD = await _planPagoRepository.ObtenerPlanPagoBase(formato.PlanPagoId);
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.ConLiquidacionDeducciones;
                        planPagoBD.UsuarioIdModificacion = usuarioId;
                        planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        await _dataContext.SaveChangesAsync();
                    }

                    #endregion Actualizar el estado al plan de pago

                    //Actualizar lista de liquidaciones anteriores(Viaticos pagados)
                    var listaDetalleLiquidacionAnterior = await _repo.ObtenerListaDetalleLiquidacionViaticosAnterior(detallePlanPago.TerceroId, pciId);

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

                    #region Actualización Solicitud de Pago

                    var solicitudPago = await _solicitudPagoRepository.ObtenerFormatoSolicitudPagoBase(formato.FormatoSolicitudPagoId);
                    solicitudPago.EstadoId = (int)EstadoSolicitudPago.ConLiquidacionDeducciones;
                    solicitudPago.UsuarioIdModificacion = usuarioId;
                    solicitudPago.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.SaveChangesAsync();

                    #endregion Actualización Solicitud de Pago

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


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> RegistrarListaDetalleLiquidacion([FromQuery(Name = "listaSolicitudPagoId")] string listaSolicitudPagoId,
                                                                            [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                            [FromQuery(Name = "seleccionarTodo")] int? seleccionarTodo,
                                                                            [FromQuery(Name = "terceroId")] int? terceroId
                                                                            )
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool esSeleccionarTodo = seleccionarTodo > 0 ? true : false;

            await _procesoLiquidacion.RegistrarListaDetalleLiquidacion(usuarioId, pciId, listaSolicitudPagoId, listIds, esSeleccionarTodo, terceroId);

            return Ok(1);

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> RechazarDetalleLiquidacion([FromQuery(Name = "solicitudPagoId")] int solicitudPagoId,
                                                                        [FromQuery(Name = "planPagoId")] int planPagoId,
                                                                        [FromQuery(Name = "mensajeRechazo")] string mensajeRechazo)
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
                    if (planPagoBD != null)
                    {
                        planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorPagar;
                        planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        planPagoBD.UsuarioIdModificacion = usuarioId;
                        await _dataContext.SaveChangesAsync();
                    }

                    //Crear nuevo plan de pago en estado rechazado
                    planNuevo = planPagoBD;
                    planNuevo.PlanPagoId = 0;
                    planNuevo.EstadoPlanPagoId = (int)EstadoPlanPago.Rechazada;
                    planNuevo.MotivoRechazo = mensajeRechazo;
                    planNuevo.UsuarioIdRegistro = usuarioId;
                    planNuevo.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    await _dataContext.PlanPago.AddAsync(planNuevo);
                    await _dataContext.SaveChangesAsync();                    

                    //Enviar email
                    var planPagoDto = await _planPagoRepository.ObtenerDetallePlanPago(planPagoId);
                    await EnviarEmail(planPagoDto, mensajeRechazo);

                    //Actualizar formato solicitud pago
                    var formatoBD = await _solicitudPagoRepository.ObtenerFormatoSolicitudPagoBase(solicitudPagoId);
                    if (formatoBD != null)
                    {
                        formatoBD.UsuarioIdModificacion = usuarioId;
                        formatoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        formatoBD.Observaciones = mensajeRechazo;
                        formatoBD.EstadoId = (int)EstadoSolicitudPago.Rechazado;
                        await _dataContext.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

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

        
          [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> RechazarLiquidacion([FromQuery(Name = "detalleLiquidacionId")] int detalleLiquidacionId,
                                                             [FromQuery(Name = "solicitudPagoId")] int solicitudPagoId,
                                                             [FromQuery(Name = "planPagoId")] int planPagoId,
                                                             [FromQuery(Name = "mensajeRechazo")] string mensajeRechazo)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);            

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (detalleLiquidacionId > 0)
                {
                    var detalleLiquidacion = await _repo.ObtenerDetalleLiquidacionBase(detalleLiquidacionId);

                    //Actualizar plan de pago existente a estado Por Pagar
                    if (detalleLiquidacion != null)
                    {
                        detalleLiquidacion.EstadoId = (int)EstadoDetalleLiquidacion.Rechazado;
                        detalleLiquidacion.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        detalleLiquidacion.UsuarioIdModificacion = usuarioId;
                        detalleLiquidacion.MotivoRechazo = mensajeRechazo;
                        await _dataContext.SaveChangesAsync();
                    }

                    //Actualizar formato solicitud pago
                    var formatoBD = await _solicitudPagoRepository.ObtenerFormatoSolicitudPagoBase(solicitudPagoId);
                    if (formatoBD != null)
                    {
                        formatoBD.UsuarioIdModificacion = usuarioId;
                        formatoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        formatoBD.EstadoId = (int)EstadoSolicitudPago.Aprobado;
                        await _dataContext.SaveChangesAsync();
                    }

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

        
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> ObtenerListaActividadesEconomicaXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            var lista = await _terceroRepository.ObtenerListaActividadesEconomicaXTercero(terceroId, pciId);
            return base.Ok(lista);
        }

        #region Archivo Cuenta Por Pagar

        [HttpGet]
        [Route("DescargarCabeceraArchivoLiquidacionCuentaPorPagar")]
        public async Task<IActionResult> DescargarCabeceraArchivoLiquidacionCuentaPorPagar([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId)
        {
            string cadena = string.Empty;
            int consecutivo = 0;
            string nombreArchivo = string.Empty;
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (!string.IsNullOrEmpty(listaLiquidacionId))
                {
                    List<int> listIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
                    List<int> listDistinct = listIds.Distinct().ToList();

                    var listaLiquidacion = await _repo.ObtenerListaDetalleLiquidacionParaArchivo(listDistinct, pciId);

                    if (listaLiquidacion != null && listaLiquidacion.Count > 0)
                    {
                        //Obtener información para el archivo
                        cadena = _procesoCreacionArchivo.ObtenerInformacionMaestroLiquidacion_ArchivoCuentaPagar(listaLiquidacion.ToList());

                        //Obtener nombre del archivo detalle
                        consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId) + 1;
                        nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                            (int)TipoDocumentoArchivo.CuentaPorPagar,
                                                            (int)TipoArchivoCuentaPorPagar.Cabecera);

                        //Registrar archivo y sus detalles
                        ArchivoDetalleLiquidacion archivo = RegistrarArchivoDetalleLiquidacion(usuarioId, pciId, listDistinct,
                                                                                                nombreArchivo, consecutivo,
                                                                                                (int)TipoDocumentoArchivo.CuentaPorPagar);
                        _dataContext.SaveChanges();

                        RegistrarDetalleArchivoLiquidacion(archivo.ArchivoDetalleLiquidacionId, listDistinct);
                        _dataContext.SaveChanges();

                        //Encoding.UTF8: Respeta las tildes en las palabras
                        byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                        MemoryStream stream = new MemoryStream(byteArray);

                        if (stream == null)
                            return NotFound();

                        await transaction.CommitAsync();
                        Response.AddFileName(archivo.Nombre);
                        return File(stream, "application/octet-stream", archivo.Nombre);
                    }
                    else
                    {
                        throw new Exception("No se pudo obtener las liquidaciones seleccionadas");
                    }
                }
                else
                {
                    throw new Exception("Debe seleccionar al menos una liquidación");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("DescargarDetalleArchivoLiquidacionCuentaPorPagar")]
        public async Task<IActionResult> DescargarDetalleArchivoLiquidacionCuentaPorPagar([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId)
        {
            string cadena = string.Empty;
            string nombreArchivo = string.Empty;
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();
            int consecutivo = 0;
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (!string.IsNullOrEmpty(listaLiquidacionId))
                {
                    List<int> listIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
                    List<int> listDistinct = listIds.Distinct().ToList();

                    var listaLiquidacion = await _repo.ObtenerListaDetalleLiquidacionParaArchivo(listDistinct, pciId);

                    //Actualizar el estado de las liquidaciones procesadas
                    await ActualizarEstadoDetalleLiquidacion(usuarioId, listDistinct);
                    await _dataContext.SaveChangesAsync();

                    //Obtener información para el archivo
                    cadena = _procesoCreacionArchivo.ObtenerInformacionDetalleLiquidacion_ArchivoCuentaPagar(listaLiquidacion.ToList());

                    //Obtener nombre del archivo detalle
                    consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId);
                    nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                        (int)TipoDocumentoArchivo.CuentaPorPagar,
                                                        (int)TipoArchivoCuentaPorPagar.Detalle);

                    //Encoding.UTF8: Respeta las tildes en las palabras
                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                    MemoryStream stream = new MemoryStream(byteArray);

                    if (stream == null)
                        return NotFound();

                    await transaction.CommitAsync();

                    Response.AddFileName(nombreArchivo);
                    return File(stream, "application/octet-stream", nombreArchivo);
                }
                else
                {
                    return BadRequest("Debe seleccionar al menos un detalle de liquidación");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Archivo Cuenta Por Pagar

        #region Archivo Obligacion Presupuestal

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerLiquidacionesParaArchivoObligacion([FromQuery(Name = "terceroId")] int? terceroId,
                                                             [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                             [FromQuery(Name = "procesado")] int? procesado,
                                                             [FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool? esProcesado = null;

            if (procesado.HasValue)
            {
                esProcesado = (procesado.Value == 1) ? (true) : (false);
            }

            var pagedList = await _repo.ObtenerLiquidacionesParaObligacionArchivo(terceroId, listIds, esProcesado, userParams);
            var listaDto = _mapper.Map<IEnumerable<FormatoCausacionyLiquidacionPagos>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }


        [HttpGet]
        [Route("ObtenerListaLiquidacionIdParaArchivo")]
        public async Task<IActionResult> ObtenerListaLiquidacionIdParaArchivo([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId,
                                                                                [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                                [FromQuery(Name = "seleccionarTodo")] int? seleccionarTodo,
                                                                                [FromQuery(Name = "terceroId")] int? terceroId,
                                                                                [FromQuery(Name = "tipoArchivoObligacionId")] int? tipoArchivoObligacionId,
                                                                                [FromQuery(Name = "conRubroFuncionamiento")] int? conRubroFuncionamiento,
                                                                                [FromQuery(Name = "conRubroUsoPresupuestal")] int? conRubroUsoPresupuestal

                                                                               )
        {
            #region Variables

            int cantidadMinimaLiquidacion = 5;
            string resultado = string.Empty;
            RespuestaSolicitudPago respuestaSolicitud = new RespuestaSolicitudPago();
            List<int> listaEstadoIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool esSeleccionarTodo = seleccionarTodo > 0 ? true : false;
            bool esRubroFuncionamiento = conRubroFuncionamiento > 0 ? true : false;
            bool esRubroUsoPresupuestal = conRubroUsoPresupuestal > 0 ? true : false;

            List<int> liquidacionIdsTotal = new List<int>();
            List<int> listaIdsConClavePresupuestal = new List<int>();
            List<int> liquidacionIdsConUsoPresupuestal = new List<int>();
            List<int> liquidacionIdsSinUsoPresupuestal = new List<int>();
            List<int> liquidacionIdsFiltroFuncionamiento = new List<int>();
            List<int> liquidacionIdsConRubroFuncionamiento = new List<int>();
            List<int> liquidacionIds = new List<int>();

            #endregion Variables

            var parametroGeneral = await _repoLista.ObtenerParametroGeneralXNombre("CantidadMinimaArchivoLiquidacion");

            if (parametroGeneral != null)
            {
                cantidadMinimaLiquidacion = Convert.ToInt16(parametroGeneral.Valor);
            }

            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            try
            {
                #region Obtener lista de liquidaciones a procesar

                if (esSeleccionarTodo)
                {
                    #region esSeleccionarTodo

                    liquidacionIdsTotal = await _repo.ObtenerLiquidacionIdsParaArchivoObligacion(pciId, terceroId, listaEstadoIds, false);

                    #endregion esSeleccionarTodo
                }
                else
                {
                    #region Selección manual

                    if (!string.IsNullOrEmpty(listaLiquidacionId))
                    {
                        liquidacionIdsTotal = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
                    }

                    #endregion Selección manual
                }

                #endregion Obtener lista de liquidaciones a procesar

                //Filtrar solo liquidaciones con clave presupuestal contable
                listaIdsConClavePresupuestal = await _repo.ObtenerLiquidacionesConClaveParaArchivoObligacion(pciId, liquidacionIdsTotal);

                #region Filtrar lista de liquidaciones Por inversión o Funcionamiento

                if (listaIdsConClavePresupuestal.Count > 0)
                {
                    liquidacionIdsConRubroFuncionamiento = await _repo.ObtenerLiquidacionIdsConRubroFuncionamiento(listaIdsConClavePresupuestal);

                    if (esRubroFuncionamiento)
                    {
                        liquidacionIdsFiltroFuncionamiento = liquidacionIdsConRubroFuncionamiento;
                    }
                    else
                    {
                        liquidacionIdsFiltroFuncionamiento = listaIdsConClavePresupuestal.Except(liquidacionIdsConRubroFuncionamiento).ToList();
                    }
                }

                #endregion Filtrar lista de liquidaciones Por inversión o codigoFuncionamiento

                #region Filtrar lista de liquidaciones por Usos Presupuestales

                if (liquidacionIdsFiltroFuncionamiento.Count > 0)
                {
                    liquidacionIdsConUsoPresupuestal = await _repo.ObtenerLiquidacionIdsConUsosPresupuestales(liquidacionIdsFiltroFuncionamiento);

                    if (esRubroUsoPresupuestal)
                    {
                        liquidacionIds = liquidacionIdsConUsoPresupuestal;
                    }
                    else
                    {
                        liquidacionIds = liquidacionIdsFiltroFuncionamiento.Except(liquidacionIdsConUsoPresupuestal).ToList();
                    }
                }

                #endregion Filtrar lista de liquidaciones por Usos Presupuestales                

                if (liquidacionIds.Count < cantidadMinimaLiquidacion)
                {
                    //return BadRequest("La cantidad mínima de liquidaciones para la creación del archivo debe ser " + cantidadMinimaLiquidacion);
                    return Ok(respuestaSolicitud);
                }
                else
                {
                    resultado = string.Join(",", liquidacionIds);
                    respuestaSolicitud.NumeroFactura = resultado;
                }
                return Ok(respuestaSolicitud);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("DescargarArchivoLiquidacionObligacion")]
        public async Task<IActionResult> DescargarArchivoLiquidacionObligacion([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId,
                                                                                [FromQuery(Name = "tipoArchivoObligacionId")] int? tipoArchivoObligacionId
                                                                               )
        {
            #region Variables

            //List<int> liquidacionIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
            List<int> liquidacionIds = new List<int>();
            string cadena = string.Empty;
            string nombreArchivo = string.Empty;
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();
            int consecutivo = 0;

            #endregion Variables

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (!string.IsNullOrEmpty(listaLiquidacionId))
                {
                    liquidacionIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();

                    switch (tipoArchivoObligacionId)
                    {
                        case (int)TipoArchivoObligacion.Cabecera:
                            {
                                #region Cabecera

                                var listaLiquidacion = await _repo.ObtenerCabeceraParaArchivoObligacion(liquidacionIds, pciId);

                                //Obtener nombre del archivo detalle
                                consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId) + 1;
                                nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                    (int)TipoDocumentoArchivo.Obligacion,
                                                                    (int)TipoArchivoObligacion.Cabecera
                                                                    );

                                if (listaLiquidacion != null && listaLiquidacion.Count > 0)
                                {
                                    //Obtener información para el archivo
                                    cadena = _procesoCreacionArchivo.ObtenerInformacionCabeceraLiquidacion_ArchivoObligacion(listaLiquidacion.ToList());

                                    //Registrar archivo y sus detalles
                                    ArchivoDetalleLiquidacion archivo = RegistrarArchivoDetalleLiquidacion(usuarioId, pciId, liquidacionIds,
                                                                                                            nombreArchivo, consecutivo,
                                                                                                            (int)TipoDocumentoArchivo.Obligacion);
                                    _dataContext.SaveChanges();

                                    RegistrarDetalleArchivoLiquidacion(archivo.ArchivoDetalleLiquidacionId, liquidacionIds);
                                    _dataContext.SaveChanges();

                                    //Encoding.UTF8: Respeta las tildes en las palabras
                                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                    MemoryStream stream = new MemoryStream(byteArray);

                                    if (stream == null)
                                        return NotFound();

                                    Response.AddFileName(archivo.Nombre);
                                    return File(stream, "application/octet-stream", archivo.Nombre);
                                }
                                else
                                {
                                    return new NoContentResult();
                                }
                                #endregion Cabecera
                            };
                        case (int)TipoArchivoObligacion.Item:
                            {
                                #region Items

                                var lista = await _repo.ObtenerItemsLiquidacionParaArchivoObligacion(liquidacionIds);

                                //Obtener nombre del archivo detalle
                                consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId);
                                nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                    (int)TipoDocumentoArchivo.Obligacion,
                                                                    (int)TipoArchivoObligacion.Item);

                                if (lista != null && lista.Count > 0)
                                {
                                    //Obtener información para el archivo
                                    cadena = _procesoCreacionArchivo.ObtenerInformacionItemsLiquidacion_ArchivoObligacion(lista.ToList());

                                    //Encoding.UTF8: Respeta las tildes en las palabras
                                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                    MemoryStream stream = new MemoryStream(byteArray);

                                    if (stream == null)
                                        return NotFound();

                                    Response.AddFileName(nombreArchivo);
                                    return File(stream, "application/octet-stream", nombreArchivo);
                                }
                                else
                                {
                                    return new NoContentResult();
                                }

                                #endregion Items
                            };
                        case (int)TipoArchivoObligacion.Deducciones:
                            {
                                #region Deducciones

                                var lista = await _repo.ObtenerDeduccionesLiquidacionParaArchivoObligacion(liquidacionIds);

                                //Obtener nombre del archivo detalle
                                consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId);
                                nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                    (int)TipoDocumentoArchivo.Obligacion,
                                                                    (int)TipoArchivoObligacion.Deducciones);

                                if (lista != null && lista.Count > 0)
                                {
                                    //Obtener información para el archivo
                                    cadena = _procesoCreacionArchivo.ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(liquidacionIds, lista.ToList());

                                    //Encoding.UTF8: Respeta las tildes en las palabras
                                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                    MemoryStream stream = new MemoryStream(byteArray);

                                    if (stream == null)
                                        return NotFound();

                                    Response.AddFileName(nombreArchivo);
                                    return File(stream, "application/octet-stream", nombreArchivo);
                                }
                                else
                                {
                                    return new NoContentResult();
                                }
                                #endregion Deducciones
                            };
                        case (int)TipoArchivoObligacion.Uso:
                            {
                                #region Usos

                                var listaUso = await _repo.ObtenerUsosParaArchivoObligacion(liquidacionIds);
                                var listaRubros = await _repo.ObtenerItemsLiquidacionParaArchivoObligacion(liquidacionIds);

                                //Obtener nombre del archivo detalle
                                consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId);
                                nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                    (int)TipoDocumentoArchivo.Obligacion,
                                                                    (int)TipoArchivoObligacion.Uso);

                                if (listaUso != null && listaUso.Count > 0)
                                {
                                    //Obtener información para el archivo
                                    cadena = _procesoCreacionArchivo.ObtenerInformacionUsosLiquidacion_ArchivoObligacion(listaUso.ToList(), listaRubros.ToList());

                                    //Encoding.UTF8: Respeta las tildes en las palabras
                                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                    MemoryStream stream = new MemoryStream(byteArray);

                                    if (stream == null)
                                        return NotFound();

                                    Response.AddFileName(nombreArchivo);
                                    return File(stream, "application/octet-stream", nombreArchivo);
                                }
                                else
                                {
                                    return new NoContentResult();
                                }

                                #endregion Usos
                            };
                        case (int)TipoArchivoObligacion.Factura:
                            {
                                #region Factura                            

                                var listaLiquidacion = await _repo.ObtenerFacturaParaArchivoObligacion(liquidacionIds);

                                //Obtener nombre del archivo detalle
                                consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion(pciId);
                                nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                    (int)TipoDocumentoArchivo.Obligacion,
                                                                    (int)TipoArchivoObligacion.Factura
                                                                    );

                                //Actualizar el estado de las liquidaciones procesadas
                                await using var transaction = await _dataContext.Database.BeginTransactionAsync();

                                await ActualizarEstadoDetalleLiquidacion(usuarioId, liquidacionIds);
                                await _dataContext.SaveChangesAsync();

                                await transaction.CommitAsync();

                                var listaIdFactura = (from l in listaLiquidacion
                                                      where l.EsFacturaElectronica == true
                                                      select l
                                                ).ToList();

                                if (listaIdFactura != null && listaIdFactura.Count > 0)
                                {
                                    //Obtener información para el archivo
                                    cadena = _procesoCreacionArchivo.ObtenerInformacionFacturaLiquidacion_ArchivoObligacion(listaLiquidacion.ToList());

                                    //Encoding.UTF8: Respeta las tildes en las palabras
                                    byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                    MemoryStream stream = new MemoryStream(byteArray);

                                    if (stream == null)
                                        return NotFound();

                                    Response.AddFileName(nombreArchivo);
                                    return File(stream, "application/octet-stream", nombreArchivo);
                                }
                                else
                                {
                                    return new NoContentResult();
                                }
                                #endregion Factura
                            };
                        default: break;
                    }
                }
                else
                {
                    return new NoContentResult();
                }
                throw new Exception("No se pudo obtener las liquidaciones seleccionadas");

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Archivo Obligacion Presupuestal

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
                                    + " de Fecha " + planPagoDto.FechaRadicadoProveedor.Value.ToString("dd-MM-yyyy")
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
            detalleLiquidacion.RubroPresupuestal = detallePlanPago.IdentificacionRubroPresupuestal != null ? detallePlanPago.IdentificacionRubroPresupuestal : string.Empty;
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
            detalleLiquidacion.TextoComprobanteContable = EliminarCaracteresEspeciales(formato.TextoComprobanteContable);

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
            detalleLiquidacion.FormatoSolicitudPagoId = formato.FormatoSolicitudPagoId;
        }

        private async Task<bool> ActualizarEstadoDetalleLiquidacion(int usuarioId, List<int> listIds)
        {

            //Actualización del detalle de liquidación
            var q = from pp in _dataContext.DetalleLiquidacion
                    where listIds.Contains(pp.DetalleLiquidacionId)
                    select pp;

            await q.BatchUpdateAsync(new DetalleLiquidacion
            {
                Procesado = true,
                UsuarioIdModificacion = usuarioId,
                FechaModificacion = _generalInterface.ObtenerFechaHoraActual()
            });

            return true;
        }

        private ArchivoDetalleLiquidacion RegistrarArchivoDetalleLiquidacion(int usuarioId, int pciId, List<int> listIds,
                                                                                string nombreArchivo, int consecutivo,
                                                                                int tipoDocumentoArchivo)
        {
            ArchivoDetalleLiquidacion archivo = new ArchivoDetalleLiquidacion();
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();

            try
            {
                archivo.FechaGeneracion = fecha;
                archivo.FechaRegistro = fecha;
                archivo.UsuarioIdRegistro = usuarioId;
                archivo.Nombre = nombreArchivo;
                archivo.CantidadRegistro = listIds.Count;
                archivo.Consecutivo = consecutivo;
                archivo.TipoDocumentoArchivo = tipoDocumentoArchivo;
                archivo.PciId = pciId;
                _dataContext.ArchivoDetalleLiquidacion.Add(archivo);
            }
            catch (Exception)
            {
                throw;
            }

            return archivo;
        }

        private string ObtenerNombreArchivo(DateTime fecha, int consecutivo, int tipoDocumentoArchivo, int tipoArchivo)
        {
            string nombre = string.Empty;
            string nombreInicial = "SIGPAA ";
            string nombreIntermedio = string.Empty;
            string nombreFinal = fecha.Year.ToString() +
                                fecha.Month.ToString().PadLeft(2, '0') +
                                fecha.Date.Day.ToString().PadLeft(2, '0') +
                                " " + consecutivo.ToString().PadLeft(4, '0');

            switch (tipoDocumentoArchivo)
            {
                case (int)TipoDocumentoArchivo.Obligacion:
                    {
                        switch (tipoArchivo)
                        {
                            case (int)TipoArchivoObligacion.Cabecera:
                                {
                                    nombreIntermedio = "Cabecera ";
                                }
                                break;
                            case (int)TipoArchivoObligacion.Item:
                                {
                                    nombreIntermedio = "Item ";
                                }
                                break;
                            case (int)TipoArchivoObligacion.Deducciones:
                                {
                                    nombreIntermedio = "Deducciones ";
                                }
                                break;
                            case (int)TipoArchivoObligacion.Uso:
                                {
                                    nombreIntermedio = "Usos ";
                                }
                                break;
                            case (int)TipoArchivoObligacion.Factura:
                                {
                                    nombreIntermedio = "Factura ";
                                }
                                break;

                            default: break;
                        }

                    }
                    break;
                case (int)TipoDocumentoArchivo.CuentaPorPagar:
                    {
                        switch (tipoArchivo)
                        {
                            case (int)TipoArchivoCuentaPorPagar.Cabecera:
                                {
                                    nombreIntermedio = "Cabecera ";
                                }
                                break;
                            case (int)TipoArchivoCuentaPorPagar.Detalle:
                                {
                                    nombreIntermedio = "Detalle ";
                                }
                                break;

                            default: break;
                        }
                    }
                    break;
                default: break;
            }
            nombre = nombreInicial + nombreIntermedio + nombreFinal;

            return nombre;
        }

        private void RegistrarDetalleArchivoLiquidacion(int archivoId, List<int> listIds)
        {
            DetalleArchivoLiquidacion detalle = null;
            List<DetalleArchivoLiquidacion> lista = new List<DetalleArchivoLiquidacion>();

            foreach (var item in listIds)
            {
                if (item > 0)
                {
                    detalle = new DetalleArchivoLiquidacion();
                    detalle.DetalleLiquidacionId = item;
                    detalle.ArchivoDetalleLiquidacionId = archivoId;
                    lista.Add(detalle);
                }
            }
            _repo.RegistrarDetalleArchivoLiquidacion(lista);
        }

        private string EliminarCaracteresEspeciales(string texto)
        {
            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            stringBuilder.Replace("ñ", "n");

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        #endregion Funciones Generales

    }
}
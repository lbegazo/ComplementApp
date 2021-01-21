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
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IListaRepository _repoLista;
        private readonly ITerceroRepository _terceroRepository;
        private readonly IProcesoCreacionArchivo _procesoCreacionArchivo;
        #endregion Dependency Injection

        public DetalleLiquidacionController(IUnitOfWork unitOfWork, IDetalleLiquidacionRepository repo,
                                     IPlanPagoRepository planPagoRepository,
                                    IMapper mapper, DataContext dataContext,
                                    IMailService mailService,
                                    IProcesoLiquidacionPlanPago procesoLiquidacion,
                                    IGeneralInterface generalInterface,
                                    IListaRepository listaRepository,
                                    ITerceroRepository terceroRepository,
                                    IProcesoCreacionArchivo procesoCreacionArchivo)
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
                List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
                bool? esProcesado = null;

                if (procesado.HasValue)
                {
                    esProcesado = (procesado.Value == 1) ? (true) : (false);
                }

                lista = await _repo.ObtenerListaDetalleLiquidacionTotal(terceroId, listIds, esProcesado);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaDetalleLiquidacion([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery(Name = "procesado")] int? procesado,
                                                              [FromQuery] UserParams userParams)
        {
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool? esProcesado = null;

            if (procesado.HasValue)
            {
                esProcesado = (procesado.Value == 1) ? (true) : (false);
            }

            var pagedList = await _repo.ObtenerListaDetalleLiquidacion(terceroId, listIds, esProcesado, userParams);
            var listaDto = _mapper.Map<IEnumerable<FormatoCausacionyLiquidacionPagos>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoCausacionyLiquidacionPago([FromQuery(Name = "planPagoId")] int planPagoId,
                                                                                    [FromQuery(Name = "valorBaseGravable")] decimal valorBaseGravable,
                                                                                    [FromQuery(Name = "actividadEconomicaId")] int? actividadEconomicaId)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {

                formato = await _procesoLiquidacion.ObtenerFormatoCausacionyLiquidacionPago(planPagoId, valorBaseGravable, actividadEconomicaId);
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


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> RegistrarListaDetalleLiquidacion([FromQuery(Name = "listaPlanPagoId")] string listaPlanPagoId,
                                                                            [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                            [FromQuery(Name = "seleccionarTodo")] int? seleccionarTodo,
                                                                            [FromQuery(Name = "terceroId")] int? terceroId
                                                                            )
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool esSeleccionarTodo = seleccionarTodo > 0 ? true : false;

            await _procesoLiquidacion.RegistrarListaDetalleLiquidacion(usuarioId, listaPlanPagoId, listIds, esSeleccionarTodo, terceroId);

            return Ok(1);

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

        [HttpGet]
        [Route("DescargarMaestroLiquidacion_ArchivoCuentaPorPagar")]
        public async Task<IActionResult> DescargarMaestroLiquidacion_ArchivoCuentaPorPagar([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId)
        {
            string cadena = string.Empty;
            int consecutivo = 0;
            string nombreArchivo = string.Empty;
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();

            try
            {
                if (!string.IsNullOrEmpty(listaLiquidacionId))
                {
                    List<int> listIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
                    List<int> listDistinct = listIds.Distinct().ToList();

                    var listaLiquidacion = await _repo.ObtenerListaDetalleLiquidacionParaArchivo(listDistinct);

                    if (listaLiquidacion != null && listaLiquidacion.Count > 0)
                    {
                        //Obtener información para el archivo
                        cadena = _procesoCreacionArchivo.ObtenerInformacionMaestroLiquidacion_ArchivoCuentaPagar(listaLiquidacion.ToList());

                        //Obtener nombre del archivo detalle
                        consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion() + 1;
                        nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                            (int)TipoDocumentoArchivo.CuentaPorPagar,
                                                            (int)TipoArchivoCuentaPorPagar.Cabecera);

                        //Registrar archivo y sus detalles
                        ArchivoDetalleLiquidacion archivo = RegistrarArchivoDetalleLiquidacion(usuarioId, listDistinct,
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
        [Route("DescargarDetalleLiquidacion_ArchivoCuentaPorPagar")]
        public async Task<IActionResult> DescargarDetalleLiquidacion_ArchivoCuentaPorPagar([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string cadena = string.Empty;
            string nombreArchivo = string.Empty;
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();
            int consecutivo = 0;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (!string.IsNullOrEmpty(listaLiquidacionId))
                {
                    List<int> listIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();
                    List<int> listDistinct = listIds.Distinct().ToList();

                    var listaLiquidacion = await _repo.ObtenerListaDetalleLiquidacionParaArchivo(listDistinct);

                    //Actualizar el estado de las liquidaciones procesadas
                    await ActualizarEstadoDetalleLiquidacion(usuarioId, listDistinct);
                    await _dataContext.SaveChangesAsync();

                    //Obtener información para el archivo
                    cadena = _procesoCreacionArchivo.ObtenerInformacionDetalleLiquidacion_ArchivoCuentaPagar(listaLiquidacion.ToList());

                    //Obtener nombre del archivo detalle
                    consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion();
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

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> ObtenerListaActividadesEconomicaXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            var lista = await _terceroRepository.ObtenerListaActividadesEconomicaXTercero(terceroId);
            return base.Ok(lista);
        }

        #region Archivo Obligacion Presupuestal

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerLiquidacionesParaArchivoObligacion([FromQuery(Name = "terceroId")] int? terceroId,
                                                             [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                             [FromQuery(Name = "procesado")] int? procesado,
                                                             [FromQuery] UserParams userParams)
        {
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
        [Route("DescargarArchivoLiquidacionObligacion")]
        public async Task<IActionResult> DescargarArchivoLiquidacionObligacion([FromQuery(Name = "listaLiquidacionId")] string listaLiquidacionId,
                                                                               [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                               [FromQuery(Name = "seleccionarTodo")] int? seleccionarTodo,
                                                                               [FromQuery(Name = "terceroId")] int? terceroId,
                                                                                [FromQuery(Name = "tipoArchivoObligacionId")] int? tipoArchivoObligacionId
                                                                               )
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<int> listaEstadoIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            bool esSeleccionarTodo = seleccionarTodo > 0 ? true : false;
            List<int> LiquidacionIds = new List<int>();
            string cadena = string.Empty;
            string nombreArchivo = string.Empty;
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();
            int consecutivo = 0;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                #region Obtener lista de liquidaciones a procesar

                if (esSeleccionarTodo)
                {
                    #region esSeleccionarTodo

                    LiquidacionIds = await _repo.ObtenerLiquidacionIdsParaArchivoObligacion(terceroId, listaEstadoIds, false);

                    #endregion esSeleccionarTodo
                }
                else
                {
                    if (!string.IsNullOrEmpty(listaLiquidacionId))
                    {
                        #region Procesar por lista de ids

                        LiquidacionIds = listaLiquidacionId.Split(',').Select(int.Parse).ToList();

                        #endregion Procesar por lista de ids
                    }
                }

                List<int> listDistinct = LiquidacionIds.Distinct().ToList();

                #endregion Obtener lista de liquidaciones a procesar

                switch (tipoArchivoObligacionId)
                {
                    case (int)TipoArchivoObligacion.Cabecera:
                        {
                            #region Cabecera

                            var listaLiquidacion = await _repo.ObtenerCabeceraParaArchivoObligacion(listDistinct);

                            //Obtener nombre del archivo detalle
                            consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion() + 1;
                            nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                (int)TipoDocumentoArchivo.Obligacion,
                                                                (int)TipoArchivoObligacion.Cabecera);

                            if (listaLiquidacion != null && listaLiquidacion.Count > 0)
                            {
                                //Obtener información para el archivo
                                cadena = _procesoCreacionArchivo.ObtenerInformacionCabeceraLiquidacion_ArchivoObligacion(listaLiquidacion.ToList());

                                //Registrar archivo y sus detalles
                                ArchivoDetalleLiquidacion archivo = RegistrarArchivoDetalleLiquidacion(usuarioId, listDistinct,
                                                                                                        nombreArchivo, consecutivo,
                                                                                                        (int)TipoDocumentoArchivo.Obligacion);
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
                                byte[] byteArray = Encoding.UTF8.GetBytes(string.Empty);
                                MemoryStream stream = new MemoryStream(byteArray);

                                if (stream == null)
                                    return NotFound();

                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }
                            #endregion Cabecera
                        };
                    case (int)TipoArchivoObligacion.Deducciones:
                        {
                            #region Deducciones

                            var lista = await _repo.ObtenerDeduccionesLiquidacionParaArchivoObligacion(listDistinct);

                            //Obtener nombre del archivo detalle
                            consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion();
                            nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                (int)TipoDocumentoArchivo.Obligacion,
                                                                (int)TipoArchivoObligacion.Deducciones);

                            if (lista != null && lista.Count > 0)
                            {
                                //Obtener información para el archivo
                                cadena = _procesoCreacionArchivo.ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(lista.ToList());

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
                                byte[] byteArray = Encoding.UTF8.GetBytes(string.Empty);
                                MemoryStream stream = new MemoryStream(byteArray);

                                if (stream == null)
                                    return NotFound();

                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }
                            #endregion Deducciones
                        };
                    case (int)TipoArchivoObligacion.Item:
                        {
                            #region Items

                            var lista = await _repo.ObtenerItemsLiquidacionParaArchivoObligacion(listDistinct);

                            //Obtener nombre del archivo detalle
                            consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion();
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

                                await transaction.CommitAsync();

                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }
                            else
                            {
                                byte[] byteArray = Encoding.UTF8.GetBytes(string.Empty);
                                MemoryStream stream = new MemoryStream(byteArray);

                                if (stream == null)
                                    return NotFound();

                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }

                            #endregion Items
                        };
                    case (int)TipoArchivoObligacion.Uso:
                        {
                            #region Usos

                            var lista = await _repo.ObtenerUsosParaArchivoObligacion(listDistinct);

                            //Obtener nombre del archivo detalle
                            consecutivo = _repo.ObtenerUltimoConsecutivoArchivoLiquidacion();
                            nombreArchivo = ObtenerNombreArchivo(fecha, consecutivo,
                                                                (int)TipoDocumentoArchivo.Obligacion,
                                                                (int)TipoArchivoObligacion.Uso);

                            if (lista != null && lista.Count > 0)
                            {
                                //Actualizar el estado de las liquidaciones procesadas
                                await ActualizarEstadoDetalleLiquidacion(usuarioId, listDistinct);
                                await _dataContext.SaveChangesAsync();

                                //Obtener información para el archivo
                                cadena = _procesoCreacionArchivo.ObtenerInformacionUsosLiquidacion_ArchivoObligacion(lista.ToList());

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
                                byte[] byteArray = Encoding.UTF8.GetBytes(string.Empty);
                                MemoryStream stream = new MemoryStream(byteArray);

                                if (stream == null)
                                    return NotFound();

                                //Response.AddFileName(archivo.Nombre);
                                //return File(stream, "application/octet-stream", archivo.Nombre);
                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }

                            #endregion Usos
                        };
                    default: break;
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

        private ArchivoDetalleLiquidacion RegistrarArchivoDetalleLiquidacion(int usuarioId, List<int> listIds,
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
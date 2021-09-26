using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ComplementApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models.ExcelDocumento;

namespace ComplementApp.API.Controllers
{
    public class DocumentoController : BaseApiController
    {

        #region Propiedades

        private readonly IDocumentoRepository _repo;
        private readonly DataContext _dataContext;
        private readonly IProcesoDocumentoExcel _documento;

        const string nombreHojaCabecera = "BD";
        const string nombreHojaDetalle = "DetallePresup";

        const string nombreHojaPlanPago = "PlanesPago";

        #endregion Propiedades

        public DocumentoController(IDocumentoRepository repo, DataContext dataContext, IProcesoDocumentoExcel documento)
        {
            _documento = documento;
            _repo = repo;
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("upload")]
        public IActionResult ActualizarBaseDeDatos()
        {

            if (Request.Form.Files.Count > 0)
            {
                using var transaction = _dataContext.Database.BeginTransaction();

                try
                {
                    IFormFile file = Request.Form.Files[0];

                    if (file == null || file.Length <= 0)
                        return BadRequest("El archivo se encuentra vacío");

                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: xlsx");

                    #region Obtener información del archivo excel

                    DataTable dtCabecera = _documento.ObtenerCabeceraDeExcel(file);
                    //DataTable dtDetalle = _documento.ObtenerDetalleDeExcel(file);
                    // DataTable dtPlanPago = _documento.ObtenerPlanPagosDeExcel(file);

                    #endregion

                    #region Mapear datos en la lista de Dtos

                    List<CDPDto> listaDocumento = _documento.obtenerListaDeCDP(dtCabecera);
                    // List<DetalleCDPDto> listaDetalle = _documento.obtenerListaDeDetalleCDP(dtDetalle);
                    // List<PlanPagoDto> listaPlanPago = _documento.obtenerListaDePlanPago(dtPlanPago);

                    #endregion

                    var result = _documento.EliminarInformacionCDP();

                    #region Insertar lista en la base de datos

                    var EsCabeceraCorrecto = _repo.InsertaCabeceraCDP(listaDocumento);
                    // var EsDetalleCorrecto = _repo.InsertaDetalleCDP(listaDetalle);
                    // var EsPlanPagoCorrecto = _repo.InsertaPlanDePago(listaPlanPago);

                    #endregion Insertar lista en la base de datos

                    if (!EsCabeceraCorrecto)
                        throw new ArgumentException("No se pudo registrar: " + nombreHojaCabecera);

                    // if (!EsDetalleCorrecto)
                    //     throw new ArgumentException("No se pudieron registrar: " + nombreHojaDetalle);

                    // if (!EsPlanPagoCorrecto)
                    //     throw new ArgumentException("No se pudo registrar:" + nombreHojaPlanPago);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest("El archivo no pudo ser enviado al servidor web");
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CargarArchivosParaSolicitudPago()
        {
            if (Request.Form.Files.Count > 0)
            {
                //using var transaction = _dataContext.Database.BeginTransaction();

                try
                {
                    IFormFile file = Request.Form.Files[0];

                    if (file == null || file.Length <= 0)
                        return BadRequest("El archivo se encuentra vacío");

                    // if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    //     return BadRequest("El archivo debe tener la extensión: pdf");

                    //transaction.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest("El archivo no pudo ser enviado al servidor web");
            }
            return Ok();
        }

        #region Carga Registro Gestion Presupuestal

        [HttpPost]
        [Route("[action]")]
        public IActionResult ActualizarDocumentosBase()
        {
            int tipoArchivo = 0;
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = null;
                IFormFile fileCdp = null;
                IFormFile fileCompromiso = null;
                IFormFile fileObligacion = null;
                IFormFile fileOrdenPago = null;

                try
                {
                    file = Request.Form.Files[0];
                    if (file == null || file.Length <= 0)
                        return BadRequest("El archivo se encuentra vacío");

                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: xlsx");

                    fileCdp = ObtenerArchivoDeLista(file, 1);
                    fileCompromiso = ObtenerArchivoDeLista(file, 2);
                    fileObligacion = ObtenerArchivoDeLista(file, 3);
                    fileOrdenPago = ObtenerArchivoDeLista(file, 4);

                    tipoArchivo = ObtenerTipoArhivo(fileCdp, fileCompromiso, fileObligacion, fileOrdenPago);
                    if (tipoArchivo == 0)
                        return BadRequest("El archivo seleccionado no se pudo procesar ya que se trata de un archivo desconocido");

                    #region Obtener información del archivo excel

                    DataTable dtCdp = _documento.ObtenerInformacionDocumentoExcel(fileCdp, 23);
                    DataTable dtCompromiso = _documento.ObtenerInformacionDocumentoExcel(fileCompromiso, 35);
                    DataTable dtObligacion = _documento.ObtenerInformacionDocumentoExcel(fileObligacion, 40);
                    DataTable dtOrdenPago = _documento.ObtenerInformacionDocumentoExcel(fileOrdenPago, 50);

                    #endregion

                    #region Mapear datos en la lista de Dtos

                    List<DocumentoCdp> listaCdp = _documento.obtenerListaDocumentoCdp(dtCdp);
                    List<DocumentoCompromiso> listaCompromiso = _documento.obtenerListaDocumentoCompromiso(dtCompromiso);
                    List<DocumentoObligacion> listaObligacion = _documento.obtenerListaDocumentoObligacion(dtObligacion);
                    List<DocumentoOrdenPago> listaOrdenPago = _documento.obtenerListaDocumentoOrdenPago(dtOrdenPago);

                    #endregion

                    using var transaction = _dataContext.Database.BeginTransaction();

                    #region Eliminar datos previos

                    if (tipoArchivo == 1)
                        _repo.EliminarDocumentoCDP();

                    if (tipoArchivo == 2)
                        _repo.EliminarDocumentoCompromiso();

                    if (tipoArchivo == 3)
                        _repo.EliminarDocumentoObligacion();

                    if (tipoArchivo == 4)
                        _repo.EliminarDocumentoOrdenPago();

                    #endregion Eliminar datos previos

                    #region Insertar lista en la base de datos

                    if (listaCdp != null && listaCdp.Count > 0)
                        _repo.InsertarListaDocumentoCDP(listaCdp);

                    if (listaCompromiso != null && listaCompromiso.Count > 0)
                        _repo.InsertarListaDocumentoCompromiso(listaCompromiso);

                    if (listaObligacion != null && listaObligacion.Count > 0)
                        _repo.InsertarListaDocumentoObligacion(listaObligacion);

                    if (listaOrdenPago != null && listaOrdenPago.Count > 0)
                        _repo.InsertarListaDocumentoOrdenPago(listaOrdenPago);

                    #endregion Insertar lista en la base de datos

                    transaction.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest("El archivo no pudo ser enviado al servidor web");
            }

            return Ok(tipoArchivo);
        }

        private IFormFile ObtenerArchivoDeLista(IFormFile archivo, int tipoArchivo)
        {
            IFormFile file = null;

            if (archivo != null)
            {
                if (tipoArchivo == 1 && archivo.FileName.ToUpper().Contains("CDP"))
                {
                    file = archivo;
                }

                if (tipoArchivo == 2 && archivo.FileName.ToUpper().Contains("COMPROMISO"))
                {
                    file = archivo;
                }

                if (tipoArchivo == 3 && archivo.FileName.ToUpper().Contains("OBLIGA"))
                {
                    file = archivo;
                }

                if (tipoArchivo == 4 && archivo.FileName.ToUpper().Contains("ORDENES"))
                {
                    file = archivo;
                }

            }
            return file;
        }

        private int ObtenerTipoArhivo(IFormFile cdp, IFormFile compromiso, IFormFile obligacion, IFormFile ordenPago)
        {
            if (cdp != null)
            {
                return 1;
            }
            if (compromiso != null)
            {
                return 2;
            }
            if (obligacion != null)
            {
                return 3;
            }
            if (ordenPago != null)
            {
                return 4;
            }
            return 0;
        }

        #endregion Carga Registro Gestion Presupuestal

    }
}
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace ComplementApp.API.Controllers
{
    public class DocumentoController : BaseApiController
    {
        #region Variable
        int pciId = 0;
        string valorPciId = string.Empty;

        const string nombreHojaCabecera = "BD";
        const string nombreHojaDetalle = "DetallePresup";

        const string nombreHojaPlanPago = "PlanesPago";
        #endregion Variable

        #region Propiedades

        private readonly IDocumentoRepository _repo;
        private readonly DataContext _dataContext;
        private readonly IProcesoDocumentoExcel _excelDocumento;
        private readonly IDocumentoService _docService;

        #endregion Propiedades

        public DocumentoController(IDocumentoRepository repo, DataContext dataContext, IProcesoDocumentoExcel documento, ICDPRepository cdpRepo, IDocumentoService docService)
        {
            _excelDocumento = documento;
            _repo = repo;
            _dataContext = dataContext;
            _docService = docService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> ActualizarBaseDeDatos()
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files[0];

                if (file == null || file.Length <= 0)
                    return BadRequest("El archivo se encuentra vacío");

                if (!Path.GetExtension(file.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: zip");

                #region Obtener información del archivo excel

                DataTable dtCabecera = _excelDocumento.ObtenerCabeceraDeExcel(file);

                #endregion

                #region Mapear datos en la lista de Dtos

                List<CDPDto> listaDocumento = _excelDocumento.obtenerListaDeCDP(dtCabecera);

                #endregion

                #region Registrar lista CDP

                 await _docService.CargarInformacionCDP(listaDocumento); 

                #endregion Registrar lista CDP
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
        public async Task<IActionResult> ActualizarDocumentosBase()
        {
            int tipoArchivo = 0;
            if (Request.Form.Files.Count > 0)
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

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

                    DataTable dtCdp = _excelDocumento.ObtenerInformacionDocumentoExcel(fileCdp, 23);
                    DataTable dtCompromiso = _excelDocumento.ObtenerInformacionDocumentoExcel(fileCompromiso, 35);
                    DataTable dtObligacion = _excelDocumento.ObtenerInformacionDocumentoExcel(fileObligacion, 40);
                    DataTable dtOrdenPago = _excelDocumento.ObtenerInformacionDocumentoExcel(fileOrdenPago, 50);

                    #endregion

                    #region Mapear datos en la lista de Dtos

                    List<DocumentoCdp> listaCdp = _excelDocumento.obtenerListaDocumentoCdp(dtCdp);
                    List<DocumentoCompromiso> listaCompromiso = _excelDocumento.obtenerListaDocumentoCompromiso(dtCompromiso);
                    List<DocumentoObligacion> listaObligacion = _excelDocumento.obtenerListaDocumentoObligacion(dtObligacion);
                    List<DocumentoOrdenPago> listaOrdenPago = _excelDocumento.obtenerListaDocumentoOrdenPago(dtOrdenPago);

                    #endregion

                    #region Insertar, Eliminar y Registrar CDP

                    await _docService.CargaDocumentoGestionPresupuestal(pciId, tipoArchivo, listaCdp, listaCompromiso, listaObligacion, listaOrdenPago);

                    #endregion Insertar, Eliminar y Registrar CDP
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

                if (tipoArchivo == 2 && archivo.FileName.ToUpper().Contains("CRP"))
                {
                    file = archivo;
                }

                if (tipoArchivo == 3 && archivo.FileName.ToUpper().Contains("OB"))
                {
                    file = archivo;
                }

                if (tipoArchivo == 4 && archivo.FileName.ToUpper().Contains("OP"))
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
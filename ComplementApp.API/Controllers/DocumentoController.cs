using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AutoMapper;
using ComplementApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Services;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
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

                    DataTable dtDetalle = _documento.ObtenerDetalleDeExcel(file);
                    DataTable dtCabecera = _documento.ObtenerCabeceraDeExcel(file);
                    DataTable dtPlanPago = _documento.ObtenerPlanPagosDeExcel(file);

                    #endregion

                    #region Mapear datos en la lista de Dtos

                    List<CDPDto> listaDocumento = _documento.obtenerListaDeCDP(dtCabecera);
                    List<DetalleCDPDto> listaDetalle = _documento.obtenerListaDeDetalleCDP(dtDetalle);
                    List<PlanPagoDto> listaPlanPago = _documento.obtenerListaDePlanPago(dtPlanPago);

                    #endregion

                    var result = _documento.EliminarInformacionCDP();     

                    #region Insertar lista en la base de datos

                    var EsCabeceraCorrecto = _repo.InsertaCabeceraCDP(listaDocumento, listaDetalle);
                    var EsDetalleCorrecto = _repo.InsertaDetalleCDP(listaDetalle);
                    var EsPlanPagoCorrecto = _repo.InsertaPlanDePago(listaPlanPago);

                    #endregion Insertar lista en la base de datos

                    if (!EsCabeceraCorrecto)
                        throw new ArgumentException("No se pudo registrar: " + nombreHojaCabecera);

                    if (!EsDetalleCorrecto)
                        throw new ArgumentException("No se pudieron registrar: " + nombreHojaDetalle);

                    if (!EsPlanPagoCorrecto)
                        throw new ArgumentException("No se pudo registrar:" + nombreHojaPlanPago);

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

        
    }
}
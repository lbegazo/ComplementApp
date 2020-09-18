using System.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;

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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        const int numeroColumnasCabecera = 26;
        const int numeroColumnasDetalle = 25;
        const int numeroColumnasPlanPago = 29;

        const string nombreHojaCabecera = "BD";
        const string nombreHojaDetalle = "DetallePresup";

        const string nombreHojaPlanPago = "PlanesPago";

        #endregion Propiedades

        private IConfiguration _configuration { get; }
        public DocumentoController(IUnitOfWork unitOfWork, IDocumentoRepository repo,
                                    IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("upload")]
        public IActionResult ActualizarBaseDeDatos()
        {

            if (Request.Form.Files.Count > 0)
            {
                var result = EliminarInformacionCDP();

                IFormFile file = Request.Form.Files[0];

                if (file == null || file.Length <= 0)
                    return BadRequest("El archivo se encuentra vacío");

                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: xlsx");

                #region Obtener información del archivo excel

                DataTable dtDetalle = ObtenerDetalleDeExcel(file);
                DataTable dtCabecera = ObtenerCabeceraDeExcel(file);
                DataTable dtPlanPago = ObtenerPlanPagosDeExcel(file);

                #endregion

                #region Mapear datos en la lista de Dtos

                List<CDPDto> listaDocumento = obtenerListaDeCDP(dtCabecera);
                List<DetalleCDPDto> listaDetalle = obtenerListaDeDetalleCDP(dtDetalle);
                List<PlanPagoDto> listaPlanPago = obtenerListaDePlanPago(dtPlanPago);

                #endregion

                #region Insertar lista en la base de datos

                var EsCabeceraCorrecto = _repo.InsertaCabeceraCDP(listaDocumento);
                var EsDetalleCorrecto = _repo.InsertaDetalleCDP(listaDetalle);
                var EsPlanPagoCorrecto = _repo.InsertaPlanDePago(listaPlanPago);

                #endregion Insertar lista en la base de datos

                if (!EsCabeceraCorrecto)
                    throw new ArgumentException("No se pudo registrar: " + nombreHojaCabecera);

                if (!EsDetalleCorrecto)
                    throw new ArgumentException("No se pudieron registrar: " + nombreHojaDetalle);

                if (!EsPlanPagoCorrecto)
                    throw new ArgumentException("No se pudo registrar:" + nombreHojaPlanPago);

            }
            else
            {
                return BadRequest("El archivo no pudo ser enviado al servidor web");
            }

            return Ok();
        }



        private static DataTable ObtenerDetalleDeExcel(IFormFile file)
        {
            DataTable dtDetalle = new DataTable();
            bool hasHeader = true;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        package.Load(stream);
                    }

                    #region Cargar Detalle

                    var wsDetalle = package.Workbook.Worksheets[nombreHojaDetalle];

                    foreach (var firstRowCell in wsDetalle.Cells[1, 1, 1, numeroColumnasDetalle])
                    {
                        dtDetalle.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsDetalle.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsDetalle.Cells[rowNum, 1, rowNum, numeroColumnasDetalle];
                        DataRow row = dtDetalle.Rows.Add();

                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        //Dejo de leer la hoja del excel
                        //Razón de la salida ID de la hoja DetallePresup
                        if (row.ItemArray[1].ToString().Equals(string.Empty))
                        {
                            dtDetalle.Rows.Remove(row);
                            break;
                        }
                    }

                    #endregion Cargar Detalle
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dtDetalle;
        }

        private static DataTable ObtenerCabeceraDeExcel(IFormFile file)
        {
            DataTable dtCabecera1 = new DataTable();
            bool hasHeader = true;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        package.Load(stream);
                    }

                    #region Cargar Cabecera

                    var wsCabecera = package.Workbook.Worksheets[nombreHojaCabecera];

                    foreach (var firstRowCell in wsCabecera.Cells[1, 1, 1, numeroColumnasCabecera])
                    {
                        dtCabecera1.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsCabecera.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsCabecera.Cells[rowNum, 1, rowNum, numeroColumnasCabecera];
                        DataRow row = dtCabecera1.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        //Dejo de leer la hoja del excel
                        //Razón de la salida: CDP de la hoja BD
                        if (row.ItemArray[1].ToString().Equals(string.Empty))
                        {
                            dtCabecera1.Rows.Remove(row);
                            break;
                        }

                    }

                    #endregion Cargar Cabecera                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dtCabecera1;
        }

        private static DataTable ObtenerPlanPagosDeExcel(IFormFile file)
        {
            DataTable dtCabecera1 = new DataTable();
            bool hasHeader = true;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        package.Load(stream);
                    }

                    #region Cargar Cabecera

                    var wsCabecera = package.Workbook.Worksheets[nombreHojaPlanPago];

                    foreach (var firstRowCell in wsCabecera.Cells[1, 1, 1, numeroColumnasPlanPago
])
                    {
                        dtCabecera1.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsCabecera.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsCabecera.Cells[rowNum, 1, rowNum, numeroColumnasPlanPago
];
                        DataRow row = dtCabecera1.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        //Dejo de leer la hoja del excel
                        //Razón de la salida: CDP de la hoja BD
                        if (row.ItemArray[1].ToString().Equals(string.Empty))
                        {
                            dtCabecera1.Rows.Remove(row);
                            break;
                        }

                    }

                    #endregion Cargar Cabecera                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dtCabecera1;
        }


        /// <summary>
        /// Consumir Servicio Rest del Banco de la República
        /// </summary>        
        /// <returns>Obtener string con la información del XML</returns>
        private async Task<string> LeerInformacionDelServicio()
        {
            string result = string.Empty;
            string url, user, pass = string.Empty;
            url = _configuration.GetSection("AppSettings:urlPresupuesto").Value;
            user = _configuration.GetSection("AppSettings:usuarioPresupuesto").Value;
            pass = _configuration.GetSection("AppSettings:contraPresupuesto").Value;

            try
            {
                var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                webrequest.Credentials = new NetworkCredential(user, pass);
                //webrequest.Accept = "text/html, application/xhtml+xml, application/xml; q=0.9, */*; q=0.8";
                webrequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");

                using (var response = await webrequest.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
            return result;
        }

        private List<CDPDto> obtenerListaDeCDP(DataTable dtCabecera)
        {
            CDPDto documento = null;
            List<CDPDto> listaDocumento = new List<CDPDto>();
            int numValue = 0;
            decimal value = 0;
            DateTime fecha;

            foreach (var row in dtCabecera.Rows)
            {
                documento = new CDPDto();

                //Instancia
                if (!(row as DataRow).ItemArray[0].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[0].ToString(), out numValue))
                        documento.Instancia = numValue;

                //Cdp
                if (!(row as DataRow).ItemArray[1].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[1].ToString(), out numValue))
                        documento.Cdp = numValue;

                //Crp
                if (!(row as DataRow).ItemArray[2].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[2].ToString(), out numValue))
                        documento.Crp = numValue;

                //Obligacion
                if (!(row as DataRow).ItemArray[3].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[3].ToString(), out numValue))
                        documento.Obligacion = numValue;

                //OrdenPago
                if (!(row as DataRow).ItemArray[4].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[4].ToString(), out numValue))
                        documento.OrdenPago = numValue;

                //Fecha
                if (!(row as DataRow).ItemArray[5].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[5].ToString(), out fecha))
                        documento.Fecha = fecha;

                documento.Detalle1 = (row as DataRow).ItemArray[7].ToString();

                //Rubro
                documento.IdentificacionRubro = (row as DataRow).ItemArray[8].ToString().Trim();

                if (!(row as DataRow).ItemArray[9].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[9].ToString(), out value))
                        documento.ValorInicial = value;

                if (!(row as DataRow).ItemArray[10].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[10].ToString(), out value))
                        documento.Operacion = value;

                if (!(row as DataRow).ItemArray[11].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[11].ToString(), out value))
                        documento.ValorTotal = value;

                if (!(row as DataRow).ItemArray[12].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[12].ToString(), out value))
                        documento.SaldoActual = value;

                documento.Detalle2 = (row as DataRow).ItemArray[13].ToString();
                documento.Detalle3 = (row as DataRow).ItemArray[14].ToString();
                documento.Detalle4 = (row as DataRow).ItemArray[16].ToString();

                if (!(row as DataRow).ItemArray[17].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[17].ToString(), out numValue))
                        documento.TipoIdentificacion = numValue;

                documento.NumeroIdentificacion = (row as DataRow).ItemArray[18].ToString();

                documento.Detalle5 = (row as DataRow).ItemArray[20].ToString();
                documento.Detalle6 = (row as DataRow).ItemArray[21].ToString();
                documento.Detalle7 = (row as DataRow).ItemArray[22].ToString();
                documento.Detalle8 = (row as DataRow).ItemArray[23].ToString();
                documento.Detalle9 = (row as DataRow).ItemArray[24].ToString();
                documento.Detalle10 = (row as DataRow).ItemArray[25].ToString();

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }

        private List<DetalleCDPDto> obtenerListaDeDetalleCDP(DataTable dtDetalle)
        {
            DetalleCDPDto detalle = null;
            List<DetalleCDPDto> listaDocumento = new List<DetalleCDPDto>();
            int numValue = 0;
            decimal value = 0;

            foreach (var row in dtDetalle.Rows)
            {
                detalle = new DetalleCDPDto();

                detalle.PcpId = (row as DataRow).ItemArray[0].ToString();

                if (!(row as DataRow).ItemArray[1].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[1].ToString(), out numValue))
                        detalle.IdArchivo = numValue;

                if (!(row as DataRow).ItemArray[2].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[2].ToString(), out numValue))
                        detalle.Cdp = numValue;

                if (!(row as DataRow).ItemArray[3].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[3].ToString(), out numValue))
                        detalle.Proy = numValue;

                if (!(row as DataRow).ItemArray[4].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[4].ToString(), out numValue))
                        detalle.Prod = numValue;

                detalle.Proyecto = (row as DataRow).ItemArray[5].ToString();
                detalle.ActividadBpin = (row as DataRow).ItemArray[6].ToString();
                detalle.PlanDeCompras = (row as DataRow).ItemArray[7].ToString();
                detalle.Responsable = (row as DataRow).ItemArray[8].ToString().Trim();
                detalle.Dependencia = (row as DataRow).ItemArray[9].ToString();
                detalle.IdentificacionRubro = (row as DataRow).ItemArray[10].ToString().Trim();

                if (!(row as DataRow).ItemArray[11].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[11].ToString(), out value))
                        detalle.ValorAct = value;

                if (!(row as DataRow).ItemArray[12].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[12].ToString(), out value))
                        detalle.SaldoAct = value;

                if (!(row as DataRow).ItemArray[13].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[13].ToString(), out value))
                        detalle.ValorCDP = value;

                if (!(row as DataRow).ItemArray[14].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[14].ToString(), out value))
                        detalle.ValorRP = value;

                if (!(row as DataRow).ItemArray[15].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[15].ToString(), out value))
                        detalle.ValorOB = value;

                if (!(row as DataRow).ItemArray[16].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[16].ToString(), out value))
                        detalle.ValorOP = value;

                detalle.AplicaContrato = (row as DataRow).ItemArray[17].ToString();

                if (!(row as DataRow).ItemArray[18].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[18].ToString(), out value))
                        detalle.SaldoTotal = value;

                if (!(row as DataRow).ItemArray[19].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[19].ToString(), out value))
                        detalle.SaldoDisponible = value;

                detalle.Area = (row as DataRow).ItemArray[20].ToString();

                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[21].ToString(), out numValue))
                        detalle.Rp = Convert.ToInt32((row as DataRow).ItemArray[21].ToString());

                if (!(row as DataRow).ItemArray[22].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[22].ToString(), out value))
                        detalle.Valor_Convenio = value;

                if (!(row as DataRow).ItemArray[23].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[23].ToString(), out numValue))
                        detalle.Convenio = numValue;

                detalle.Decreto = (row as DataRow).ItemArray[24].ToString();

                listaDocumento.Add(detalle);
            }
            return listaDocumento;
        }

        private List<PlanPagoDto> obtenerListaDePlanPago(DataTable dtPlanPago)
        {
            PlanPagoDto documento = null;
            List<PlanPagoDto> listaDocumento = new List<PlanPagoDto>();
            int numValue = 0;
            decimal value = 0;
            DateTime fecha;

            foreach (var row in dtPlanPago.Rows)
            {
                documento = new PlanPagoDto();

                //Cdp
                if (!(row as DataRow).ItemArray[1].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[1].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Cdp = numValue;

                //Crp
                if (!(row as DataRow).ItemArray[2].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[2].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Crp = numValue;

                //AnioPago
                if (!(row as DataRow).ItemArray[3].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[3].ToString(), out numValue))
                        documento.AnioPago = numValue;

                //MesPago
                if (!(row as DataRow).ItemArray[4].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[4].ToString(), out numValue))
                        documento.MesPago = numValue;

                //ValorInicial
                if (!(row as DataRow).ItemArray[5].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[5].ToString(), out value))
                        if (value > 0)
                            documento.ValorInicial = value;

                //ValorAdicion
                if (!(row as DataRow).ItemArray[6].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[6].ToString(), out value))
                        if (value > 0)
                            documento.ValorAdicion = value;

                //ValorAPagar
                if (!(row as DataRow).ItemArray[7].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[7].ToString(), out value))
                        if (value > 0)
                            documento.ValorAPagar = value;

                //ValorPagado
                if (!(row as DataRow).ItemArray[8].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[8].ToString(), out value))
                        if (value > 0)
                            documento.ValorPagado = value;

                documento.EstadoPlanPago = (row as DataRow).ItemArray[9].ToString();
                documento.ViaticosDescripcion = (row as DataRow).ItemArray[10].ToString();

                //TipoIdentificacionTercero
                if (!(row as DataRow).ItemArray[11].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[11].ToString(), out numValue))
                        documento.TipoIdentificacionTercero = numValue;
                //IdentificacionTercero
                documento.IdentificacionTercero = (row as DataRow).ItemArray[12].ToString();

                //NumeroPago
                if (!(row as DataRow).ItemArray[13].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[13].ToString(), out numValue))
                        if (numValue > 0)
                            documento.NumeroPago = numValue;

                //IdentificacionRubroPresupuestal
                documento.IdentificacionRubroPresupuestal = (row as DataRow).ItemArray[14].ToString();
                //IdentificacionUsoPresupuestal
                documento.IdentificacionUsoPresupuestal = (row as DataRow).ItemArray[15].ToString();

                //Proveedor
                documento.NumeroRadicadoProveedor = (row as DataRow).ItemArray[16].ToString();
                if (!(row as DataRow).ItemArray[17].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[17].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaRadicadoProveedor = fecha;

                //Supervisor
                documento.NumeroRadicadoSupervisor = (row as DataRow).ItemArray[18].ToString();
                if (!(row as DataRow).ItemArray[19].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[19].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaRadicadoSupervisor = fecha;

                //Factura
                documento.NumeroFactura = (row as DataRow).ItemArray[20].ToString();
                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[21].ToString(), out value))
                        if (value > 0)
                            documento.ValorFacturado = value;
                //FechaFactura
                if (!(row as DataRow).ItemArray[23].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[23].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaFactura = fecha;

                //Observaciones
                documento.Observaciones = (row as DataRow).ItemArray[22].ToString();

                //Obligacion
                if (!(row as DataRow).ItemArray[24].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[24].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Obligacion = numValue;

                //OrdenPago
                if (!(row as DataRow).ItemArray[25].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[25].ToString(), out numValue))
                        if (numValue > 0)
                            documento.OrdenPago = numValue;

                //EstadoOrdenPago
                documento.EstadoOrdenPago = (row as DataRow).ItemArray[26].ToString();
                //FechaOrdenPago 
                if (!(row as DataRow).ItemArray[27].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[27].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaOrdenPago = fecha;

                //DiasAlPago
                if (!(row as DataRow).ItemArray[28].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[28].ToString(), out numValue))
                        if (numValue > 0)
                            documento.DiasAlPago = numValue;

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }

         private bool EliminarInformacionCDP()
        {
            var resultado = false;
            resultado = this._repo.EliminarCabeceraCDP();

            if (resultado)
                resultado = this._repo.EliminarDetalleCDP();

            return resultado;
        }

        private static DataTable ObtenerDetalleDeExcel(string path)
        {
            DataTable dtDetalle = new DataTable();
            bool hasHeader = true;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        package.Load(stream);
                    }

                    #region Cargar Detalle

                    var wsDetalle = package.Workbook.Worksheets["DetallePresup"];

                    foreach (var firstRowCell in wsDetalle.Cells[1, 1, 1, 23])
                    {
                        dtDetalle.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsDetalle.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsDetalle.Cells[rowNum, 1, rowNum, 23];
                        DataRow row = dtDetalle.Rows.Add();

                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        //Me fijo en el ID de la hoja DetallePresup
                        if (row.ItemArray[1].ToString().Equals(string.Empty))
                        {
                            dtDetalle.Rows.Remove(row);
                            break;
                        }
                    }

                    #endregion Cargar Detalle
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dtDetalle;
        }

        private static DataTable ObtenerCabeceraDeExcel(string path)
        {
            DataTable dtCabecera1 = new DataTable();
            bool hasHeader = true;

            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        package.Load(stream);
                    }

                    #region Cargar Cabecera

                    var wsCabecera = package.Workbook.Worksheets["BD"];

                    foreach (var firstRowCell in wsCabecera.Cells[1, 1, 1, numeroColumnasCabecera])
                    {
                        dtCabecera1.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsCabecera.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsCabecera.Cells[rowNum, 1, rowNum, numeroColumnasCabecera];
                        DataRow row = dtCabecera1.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Value;
                        }
                        //Me fijo en el CDP de la hoja BD
                        if (row.ItemArray[3].ToString().Equals(string.Empty))
                        {
                            dtCabecera1.Rows.Remove(row);
                            break;
                        }

                    }

                    #endregion Cargar Cabecera                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dtCabecera1;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Microsoft.Extensions.Configuration;
using ComplementApp.API.Data;
using System.Net;
using ComplementApp.API.Models;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;

namespace ComplementApp.API.Services
{
    public class ProcesoDocumentoExcel : IProcesoDocumentoExcel
    {
        #region Propiedades

        private readonly IDocumentoRepository _repo;
        private readonly DataContext _dataContext;
        const int maxLongitudDetalle = 250;
        const int numeroColumnasCabecera = 27;
        const int numeroColumnasDetalle = 25;
        const int numeroColumnasPlanPago = 29;
        const string nombreHojaCabecera = "BD";
        const string nombreHojaDetalle = "DetallePresup";
        const string nombreHojaPlanPago = "PlanesPago";
        private IConfiguration _configuration { get; }

        #endregion Propiedades

        public ProcesoDocumentoExcel(IDocumentoRepository repo, IConfiguration configuration, DataContext dataContext)
        {
            _repo = repo;
            _configuration = configuration;
            _dataContext = dataContext;
        }

        public DataTable ObtenerDetalleDeExcel(IFormFile file)
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

                    if (wsDetalle != null)
                    {
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
                    }
                    #endregion Cargar Detalle
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtDetalle;
        }

        public DataTable ObtenerCabeceraDeExcel(IFormFile file)
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

                    if (wsCabecera != null)
                    {
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
                    }

                    #endregion Cargar Cabecera                    
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtCabecera1;
        }

        public DataTable ObtenerPlanPagosDeExcel(IFormFile file)
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

                    if (wsCabecera != null)
                    {
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
                    }
                    #endregion Cargar Cabecera                    
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtCabecera1;
        }

        /// <summary>
        /// Consumir Servicio Rest del Banco de la República
        /// </summary>        
        /// <returns>Obtener string con la información del XML</returns>
        public async Task<string> LeerInformacionDelServicio()
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
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public List<CDPDto> obtenerListaDeCDP(DataTable dtCabecera)
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

                documento.Detalle1 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[7].ToString(), maxLongitudDetalle);

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

                documento.Detalle2 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[13].ToString(), maxLongitudDetalle);
                documento.Detalle3 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[14].ToString(), maxLongitudDetalle);
                documento.Detalle4 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[16].ToString(), 4000);

                if (!(row as DataRow).ItemArray[17].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[17].ToString(), out numValue))
                        documento.TipoIdentificacionTercero = numValue;

                documento.NumeroIdentificacionTercero = (row as DataRow).ItemArray[18].ToString();

                documento.Detalle5 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[20].ToString(), maxLongitudDetalle);
                documento.Detalle6 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[21].ToString(), maxLongitudDetalle);
                documento.Detalle7 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[22].ToString(), maxLongitudDetalle);
                documento.Detalle8 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[23].ToString(), maxLongitudDetalle);
                documento.Detalle9 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[24].ToString(), maxLongitudDetalle);
                documento.Detalle10 = this.ObtenerCadenaLimitada((row as DataRow).ItemArray[25].ToString(), maxLongitudDetalle);
                //PCI
                documento.Pci = (row as DataRow).ItemArray[26].ToString();

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }

        public List<DetalleCDPDto> obtenerListaDeDetalleCDP(DataTable dtDetalle)
        {
            DetalleCDPDto detalle = null;
            List<DetalleCDPDto> listaDocumento = new List<DetalleCDPDto>();
            int numValue = 0;
            decimal value = 0;
            bool aplicaContrato = false;

            foreach (var row in dtDetalle.Rows)
            {
                aplicaContrato = false;
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
                detalle.RubroPresupuestal = new RubroPresupuestal();
                detalle.RubroPresupuestal.Identificacion = (row as DataRow).ItemArray[10].ToString().Trim();

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

                if ((row as DataRow).ItemArray[17].ToString() == "SI")
                {
                    aplicaContrato = true;
                }

                detalle.AplicaContrato = aplicaContrato;

                if (!(row as DataRow).ItemArray[18].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[18].ToString(), out value))
                        detalle.SaldoTotal = value;

                if (!(row as DataRow).ItemArray[19].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[19].ToString(), out value))
                        detalle.SaldoDisponible = value;

                detalle.Area = (row as DataRow).ItemArray[20].ToString();

                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[21].ToString(), out numValue))
                        detalle.Crp = Convert.ToInt32((row as DataRow).ItemArray[21].ToString());

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

        public List<PlanPagoDto> obtenerListaDePlanPago(DataTable dtPlanPago)
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
                            documento.SaldoDisponible = value;

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

        public bool EliminarInformacionCDP()
        {
            var resultado = false;
            resultado = this._repo.EliminarCabeceraCDP();
                        
            return resultado;
        }

        private string ObtenerCadenaLimitada(string cadena, int limite)
        {
            if (!string.IsNullOrEmpty(cadena))
            {
                if (cadena.Length > limite)
                {
                    return cadena.Substring(0, limite - 1);
                }
                return cadena;
            }
            return string.Empty;
        }
    }
}
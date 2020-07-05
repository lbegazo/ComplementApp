using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace ComplementApp.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        #region Propiedades
        private readonly IDocumentoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Propiedades

        private IConfiguration _configuration { get; }
        public DocumentoController(IUnitOfWork unitOfWork, IDocumentoRepository repo, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult ActualizarBaseDeDatos(string path)
        {
            var result = EliminarinformacionCDP();

            if (result)
            {
                path = _configuration.GetSection("AppSettings:pathArchivo").Value;
                DataTable dtDetalle = ObtenerDetalleDeExcel(path);
                DataTable dtCabecera = ObtenerCabeceraDeExcel(path);

                #region Mapear listas

                List<CDP> listaDocumento = obtenerListaDeCDP(dtCabecera);
                List<DetalleCDP> listaDetalle = obtenerListaDeDetalleCDP(dtDetalle);

                #endregion Mapear listas

                _repo.InsertaCabeceraCDP(listaDocumento);
                _repo.InsertaDetalleCDP(listaDetalle);
            }
            return Ok();
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

                    foreach (var firstRowCell in wsCabecera.Cells[1, 1, 1, 10])
                    {
                        dtCabecera1.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= wsCabecera.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = wsCabecera.Cells[rowNum, 1, rowNum, 10];
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

        private List<CDP> obtenerListaDeCDP(DataTable dtCabecera)
        {
            CDP documento = null;
            List<CDP> listaDocumento = new List<CDP>();
            int numValue = 0;
            decimal value = 0;
            DateTime fecha;

            foreach (var row in dtCabecera.Rows)
            {
                documento = new CDP();
                documento.Dependencia = (row as DataRow).ItemArray[0].ToString();

                if (!(row as DataRow).ItemArray[1].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[1].ToString(), out numValue))
                        documento.Proy = numValue;

                if (!(row as DataRow).ItemArray[2].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[2].ToString(), out numValue))
                        documento.Pro = numValue;

                if (!(row as DataRow).ItemArray[3].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[3].ToString(), out numValue))
                        documento.Cdp = numValue;

                if (!(row as DataRow).ItemArray[4].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[4].ToString(), out fecha))
                        documento.Fecha = fecha;

                documento.Estado = (row as DataRow).ItemArray[5].ToString();
                documento.Rubro = (row as DataRow).ItemArray[6].ToString();

                if (!(row as DataRow).ItemArray[7].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[7].ToString(), out value))
                        documento.Valor = value;

                if (!(row as DataRow).ItemArray[8].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[8].ToString(), out value))
                        documento.Saldo = value;

                documento.Tipo = (row as DataRow).ItemArray[9].ToString();

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }

        private List<DetalleCDP> obtenerListaDeDetalleCDP(DataTable dtDetalle)
        {
            DetalleCDP detalle = null;
            List<DetalleCDP> listaDocumento = new List<DetalleCDP>();
            int numValue = 0;
            decimal value = 0;

            foreach (var row in dtDetalle.Rows)
            {
                detalle = new DetalleCDP();
                if (!(row as DataRow).ItemArray[0].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[0].ToString(), out numValue))
                        detalle.Crp = numValue;

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
                detalle.Responsable = (row as DataRow).ItemArray[8].ToString();
                detalle.Dependencia = (row as DataRow).ItemArray[9].ToString();
                detalle.Rubro = (row as DataRow).ItemArray[10].ToString();

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

                detalle.Contrato = (row as DataRow).ItemArray[17].ToString();

                if (!(row as DataRow).ItemArray[18].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[18].ToString(), out value))
                        detalle.SaldoTotal = value;

                if (!(row as DataRow).ItemArray[19].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[19].ToString(), out value))
                        detalle.SaldoDisponible = value;

                detalle.Area = (row as DataRow).ItemArray[20].ToString();

                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[21].ToString(), out numValue))
                        detalle.Paa = Convert.ToInt32((row as DataRow).ItemArray[21].ToString());

                if (!(row as DataRow).ItemArray[22].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[22].ToString(), out numValue))
                        detalle.IdSofi = numValue;


                listaDocumento.Add(detalle);
            }

            return listaDocumento;
        }

        private bool EliminarinformacionCDP()
        {
            var resultado = false;
            resultado = this._repo.EliminarCabeceraCDP();

            if (resultado)
                resultado = this._repo.EliminarDetalleCDP();

            return resultado;
        }
    }
}
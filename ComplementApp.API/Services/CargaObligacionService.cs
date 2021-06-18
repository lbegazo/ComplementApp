using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ComplementApp.API.Data;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace ComplementApp.API.Services
{
    public class CargaObligacionService : ICargaObligacionService
    {

        private readonly ICargaObligacionRepository _repo;

        private readonly DataContext _dataContext;
        private const int numeroColumnasCabecera = 40;

        public CargaObligacionService(ICargaObligacionRepository repo, DataContext dataContext)
        {
            _repo = repo;
            _dataContext = dataContext;
        }


        public bool EliminarCargaObligacion()
        {
            var resultado = false;
            resultado = this._repo.EliminarCargaObligacion();
            return resultado;
        }

        public DataTable ObtenerInformacionDeExcel(IFormFile file)
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

                    var wsCabecera = package.Workbook.Worksheets[0];

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


        public List<CargaObligacion> ObtenerListaCargaObligacion(DataTable dt)
        {
            CargaObligacion documento = null;
            List<CargaObligacion> listaDocumento = new List<CargaObligacion>();
            long numValueLong = 0;
            int numValue = 0;
            decimal value = 0;
            DateTime fecha;

            foreach (var row in dt.Rows)
            {
                documento = new CargaObligacion();

                //NumeroDocumento
                if (!(row as DataRow).ItemArray[0].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[0].ToString(), out numValue))
                        if (numValue > 0)
                            documento.NumeroDocumento = numValue;

                //FechaRegistro
                if (!(row as DataRow).ItemArray[1].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[1].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaRegistro = fecha;

                //FechaRegistro
                if (!(row as DataRow).ItemArray[2].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[2].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaCreacion = fecha;

                //Estado
                documento.Estado = (row as DataRow).ItemArray[3].ToString().Trim();

                //ValorActual
                if (!(row as DataRow).ItemArray[4].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[4].ToString(), out value))
                        if (value > 0)
                            documento.ValorActual = value;

                //ValorDeduccion
                if (!(row as DataRow).ItemArray[5].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[5].ToString(), out value))
                        if (value > 0)
                            documento.ValorDeduccion = value;

                //ValorObligadoNoOrdenado
                if (!(row as DataRow).ItemArray[6].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[6].ToString(), out value))
                        if (value > 0)
                            documento.ValorObligadoNoOrdenado = value;

                //TipoIdentificacion
                documento.TipoIdentificacion = (row as DataRow).ItemArray[7].ToString().Trim();

                //NumeroIdentificacion
                documento.NumeroIdentificacion = (row as DataRow).ItemArray[8].ToString().Trim();

                //NumeroIdentificacion
                documento.NombreRazonSocial = (row as DataRow).ItemArray[9].ToString().Trim();

                //MedioPago
                documento.MedioPago = (row as DataRow).ItemArray[10].ToString().Trim();

                //TipoCuenta
                documento.TipoCuenta = (row as DataRow).ItemArray[11].ToString().Trim();

                //NumeroCuenta 
                documento.NumeroCuenta = (row as DataRow).ItemArray[12].ToString().Trim();

                //EstadoCuenta 
                documento.EstadoCuenta = (row as DataRow).ItemArray[13].ToString().Trim();

                //EntidadNit 
                documento.EntidadNit = (row as DataRow).ItemArray[14].ToString().Trim();

                //EntidadDescripcion 
                documento.EntidadDescripcion = (row as DataRow).ItemArray[15].ToString().Trim();
                //EntidadDescripcion 
                documento.Dependencia = (row as DataRow).ItemArray[16].ToString().Trim();
                //EntidadDescripcion 
                documento.DependenciaDescripcion = (row as DataRow).ItemArray[17].ToString().Trim();
                //RubroIdentificacion 
                documento.RubroIdentificacion = (row as DataRow).ItemArray[18].ToString().Trim();
                //RubroDescripcion 
                documento.RubroDescripcion = (row as DataRow).ItemArray[19].ToString().Trim();

                //ValorInicial
                if (!(row as DataRow).ItemArray[20].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[20].ToString(), out value))
                        if (value > 0)
                            documento.ValorInicial = value;

                //ValorOperacion
                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[21].ToString(), out value))
                        if (value > 0)
                            documento.ValorOperacion = value;

                //ValorActual2
                if (!(row as DataRow).ItemArray[22].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[22].ToString(), out value))
                        if (value > 0)
                            documento.ValorActual2 = value;

                //SaldoPorUtilizar
                if (!(row as DataRow).ItemArray[23].ToString().Equals(string.Empty))
                    if (decimal.TryParse((row as DataRow).ItemArray[23].ToString(), out value))
                        if (value > 0)
                            documento.SaldoPorUtilizar = value;

                //FuenteFinanciacion
                documento.FuenteFinanciacion = (row as DataRow).ItemArray[24].ToString();
                //SituacionFondo
                documento.SituacionFondo = (row as DataRow).ItemArray[25].ToString();
                //RecursoPresupuestal
                documento.RecursoPresupuestal = (row as DataRow).ItemArray[26].ToString();
                //Concepto
                documento.Concepto = (row as DataRow).ItemArray[27].ToString();

                //SolicitudCdp
                if (!(row as DataRow).ItemArray[28].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[28].ToString(), out numValue))
                        if (numValue > 0)
                            documento.SolicitudCdp = numValue;

                //Cdp
                if (!(row as DataRow).ItemArray[29].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[29].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Cdp = numValue;

                //Compromiso
                if (!(row as DataRow).ItemArray[30].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[30].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Compromiso = numValue;

                //CuentaPorPagar
                if (!(row as DataRow).ItemArray[31].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[31].ToString(), out numValue))
                        if (numValue > 0)
                            documento.CuentaPorPagar = numValue;

                //FechaCuentaPorPagar
                if (!(row as DataRow).ItemArray[32].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[32].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaCuentaPorPagar = fecha;

                //Obligacion
                if (!(row as DataRow).ItemArray[33].ToString().Equals(string.Empty))
                    if (Int32.TryParse((row as DataRow).ItemArray[33].ToString(), out numValue))
                        if (numValue > 0)
                            documento.Obligacion = numValue;

                //OrdenPago
                if (!(row as DataRow).ItemArray[34].ToString().Equals(string.Empty))
                    if (long.TryParse((row as DataRow).ItemArray[34].ToString(), out numValueLong))
                        if (numValueLong > 0)
                            documento.OrdenPago = numValueLong;

                //Reintegro
                documento.Reintegro = (row as DataRow).ItemArray[35].ToString();

                //FechaDocSoporteCompromiso
                if (!(row as DataRow).ItemArray[36].ToString().Equals(string.Empty))
                    if (DateTime.TryParse((row as DataRow).ItemArray[36].ToString(), out fecha))
                        if (fecha != DateTime.MinValue)
                            documento.FechaDocSoporteCompromiso = fecha;

                //TipoDocSoporteCompromiso
                documento.TipoDocSoporteCompromiso = (row as DataRow).ItemArray[37].ToString();

                //NumeroDocSoporteCompromiso
                documento.NumeroDocSoporteCompromiso = (row as DataRow).ItemArray[38].ToString();

                //ObjetoCompromiso
                documento.ObjetoCompromiso = (row as DataRow).ItemArray[39].ToString();

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }
    }
}
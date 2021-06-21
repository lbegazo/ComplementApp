using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
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
        private readonly IListaRepository _repoLista;

        private const int numeroColumnasCabecera = 40;

        public CargaObligacionService(ICargaObligacionRepository repo, DataContext dataContext, IListaRepository listaRepository)
        {
            _repo = repo;
            _dataContext = dataContext;
            _repoLista = listaRepository;
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

        public async Task<List<CargaObligacion>> ObtenerListaCargaObligacion(int pciId, DataTable dt)
        {
            CargaObligacion documento = null;
            List<CargaObligacion> listaDocumento = new List<CargaObligacion>();
            long numValueLong = 0;
            int numValue = 0;
            decimal value = 0;
            DateTime fecha;
            string valor = string.Empty;
            RubroPresupuestal rubroPresupuestal = null;
            ValorSeleccion fuenteFinanciacion = null;
            ValorSeleccion situacionFondo = null;
            ValorSeleccion recursoPresupuestal = null;

            var listaRubroPresupuestal = (await _repoLista.ObtenerListaRubroPresupuestal(string.Empty, string.Empty)).ToList();
            var listaFuenteFinanciacion = (await _repoLista.ObtenerListaXTipo(TipoLista.FuenteFinanciacion)).ToList();
            var listaSituacionFondo = (await _repoLista.ObtenerListaXTipo(TipoLista.SituacionFondo)).ToList();
            var listaRecursoPresupuestal = (await _repoLista.ObtenerListaXTipo(TipoLista.RecursoPresupuestal)).ToList();

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
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorActual = value;
                }

                //ValorDeduccion
                if (!(row as DataRow).ItemArray[5].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorDeduccion = value;
                }

                //ValorObligadoNoOrdenado
                if (!(row as DataRow).ItemArray[6].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorObligadoNoOrdenado = value;
                }

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

                rubroPresupuestal = obtenerRubroPresupuestal((row as DataRow).ItemArray[18].ToString().Trim(), listaRubroPresupuestal);

                if (rubroPresupuestal != null)
                {
                    documento.RubroPresupuestalId = rubroPresupuestal.RubroPresupuestalId;
                }

                //ValorInicial
                if (!(row as DataRow).ItemArray[20].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorInicial = value;
                }

                //ValorOperacion
                if (!(row as DataRow).ItemArray[21].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorOperacion = value;
                }

                //ValorActual2
                if (!(row as DataRow).ItemArray[22].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.ValorActual2 = value;
                }

                //SaldoPorUtilizar
                if (!(row as DataRow).ItemArray[23].ToString().Equals(string.Empty))
                {
                    valor = (row as DataRow).ItemArray[4].ToString().Replace(",", "");
                    if (decimal.TryParse(valor, out value))
                        if (value > 0)
                            documento.SaldoPorUtilizar = value;
                }

                //FuenteFinanciacion
                fuenteFinanciacion = listaFuenteFinanciacion.Where(f => f.Nombre.ToLower() == (row as DataRow).ItemArray[24].ToString().ToLower()).FirstOrDefault();
                if (fuenteFinanciacion != null)
                {
                    documento.FuenteFinanciacionId = fuenteFinanciacion.Id;
                }
                //SituacionFondo
                situacionFondo = listaSituacionFondo.Where(f => f.Nombre.ToLower() == (row as DataRow).ItemArray[25].ToString().ToLower()).FirstOrDefault();
                if (situacionFondo != null)
                {
                    documento.SituacionFondoId = situacionFondo.Id;
                }
                //RecursoPresupuestal
                recursoPresupuestal = listaRecursoPresupuestal.Where(f => f.Nombre.ToLower() == (row as DataRow).ItemArray[26].ToString().ToLower()).FirstOrDefault();
                if (situacionFondo != null)
                {
                    documento.RecursoPresupuestalId = recursoPresupuestal.Id;
                }

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
                documento.PciId = pciId;

                listaDocumento.Add(documento);
            }

            return listaDocumento;
        }

        public string ObtenerInformacionOrdenPagoArchivoCabecera(List<CargaObligacionDto> lista)
        {
            int consecutivo = 1;
            StringBuilder sbBody = new StringBuilder();

            foreach (var item in lista)
            {
                sbBody.Append(consecutivo);
                sbBody.Append("|");
                sbBody.Append(item.Pci.Identificacion);
                sbBody.Append("|");
                sbBody.Append(item.FechaRegistro.ToString("yyyy-MM-dd"));
                sbBody.Append("|");
                sbBody.Append(item.Obligacion);
                sbBody.Append("|");
                sbBody.Append(item.CodigoDependenciaAfectacionPac);
                sbBody.Append("|");
                sbBody.Append(item.CodigoPosicionPac);
                sbBody.Append("|");
                sbBody.Append(item.FechaPago.ToString("yyyy-MM-dd"));
                sbBody.Append("|");
                sbBody.Append(item.ValorActual.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append(item.CodigoTipoBeneficiario);
                sbBody.Append("|");
                sbBody.Append(item.MedioPago);
                sbBody.Append("|");
                sbBody.Append(item.CodigoPciTesoreria);
                sbBody.Append("|");
                sbBody.Append(item.Tercero.TipoDocumentoIdentidad);
                sbBody.Append("|");
                sbBody.Append(item.Tercero.NumeroIdentificacion);
                sbBody.Append("|");
                sbBody.Append(item.NumeroCuenta);
                sbBody.Append("|");
                sbBody.Append(item.TipoCuenta);
                sbBody.Append("|");
                sbBody.Append(item.FechaLimitePago.ToString("yyyy-MM-dd"));
                sbBody.Append("|");
                sbBody.Append(item.TipoDocSoporteCompromiso);
                sbBody.Append("|");
                sbBody.Append(item.NumeroDocSoporteCompromiso);
                sbBody.Append("|");
                sbBody.Append(item.FechaDocSoporteCompromiso.ToString("yyyy-MM-dd"));
                //Expedidor vacío
                sbBody.Append("||");
                sbBody.Append(item.NombreFuncionario);
                sbBody.Append("|");
                sbBody.Append(item.CargoFuncionario);
                sbBody.Append("|");
                sbBody.Append(item.ObjetoCompromiso);
                sbBody.Append("|");
                sbBody.Append(Environment.NewLine);
                consecutivo++;
            }
            return sbBody.ToString();
        }

        public string ObtenerInformacionOrdenPagoArchivoDetalle(List<CargaObligacionDto> listaTotal)
        {
            List<CargaObligacionDto> lista = null;
            int consecutivoCabecera = 1;
            int consecutivoInterno = 1;
            StringBuilder sbBody = new StringBuilder();

            var listaAgrupada = (from l in listaTotal
                                 group l by new { l.Obligacion }
                                 into grp
                                 select new
                                 {
                                     grp.Key.Obligacion
                                 });

            var listaIds = listaAgrupada.Select(s => s.Obligacion).ToList();

            foreach (var item in listaIds)
            {
                lista = listaTotal.Where(x => x.Obligacion == item).ToList();

                foreach (var itemInterno in lista)
                {
                    sbBody.Append(consecutivoCabecera);
                    sbBody.Append("|");
                    sbBody.Append(consecutivoInterno);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.Dependencia);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.RubroPresupuestal.Identificacion);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.RecursoPresupuestal.Codigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.FuenteFinanciacion.Codigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.SituacionFondo.Codigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.ValorActual2.ToString().Replace(".", ","));
                    sbBody.Append(Environment.NewLine);

                    consecutivoInterno++;
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }


        private RubroPresupuestal obtenerRubroPresupuestal(string Identificacion, List<RubroPresupuestal> lista)
        {
            return lista.Where(x => x.Identificacion == Identificacion).FirstOrDefault();
        }
    }
}
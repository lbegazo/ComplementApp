using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Data;

namespace ComplementApp.API.Services
{
    public class ProcesoCreacionArchivo : IProcesoCreacionArchivo
    {
        private readonly IGeneralInterface _generalInterface;
        private readonly DataContext _dataContext;
        private readonly IDetalleLiquidacionRepository _repo;

        public ProcesoCreacionArchivo(DataContext dataContext, IGeneralInterface generalInterface, IDetalleLiquidacionRepository repo)
        {
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _repo = repo;
        }

        #region Cuenta Por Pagar
        public string ObtenerInformacionMaestroLiquidacion_ArchivoCuentaPagar(List<DetalleLiquidacionParaArchivo> lista)
        {
            int consecutivo = 1;
            StringBuilder sbBody = new StringBuilder();

            foreach (var item in lista)
            {
                sbBody.Append(item.PCI);
                sbBody.Append("|");
                sbBody.Append(consecutivo);
                sbBody.Append("|");
                sbBody.Append(item.FechaActual);
                sbBody.Append("|");
                sbBody.Append(item.TipoIdentificacion.ToString().PadLeft(2, '0'));
                sbBody.Append("|");
                sbBody.Append(item.NumeroIdentificacion);
                sbBody.Append("|");
                sbBody.Append(item.Crp);
                sbBody.Append("|");
                sbBody.Append(item.TipoCuentaXPagarCodigo);
                sbBody.Append("|");
                sbBody.Append(item.TotalACancelar.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append("0");
                sbBody.Append("|");
                sbBody.Append(item.ValorIva.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append(item.TextoComprobanteContable);
                sbBody.Append("|");
                sbBody.Append(item.TipoDocumentoSoporteCodigo);
                sbBody.Append("|");
                sbBody.Append(item.NumeroFactura);
                sbBody.Append("|");
                sbBody.Append(item.FechaActual);
                sbBody.Append("|");
                sbBody.Append(item.ConstanteNumero);
                sbBody.Append("|");
                sbBody.Append(item.NombreSupervisor);
                sbBody.Append("|");
                sbBody.Append(item.ConstanteCargo);
                sbBody.Append("|||");
                sbBody.Append(Environment.NewLine);
                consecutivo++;
            }

            return sbBody.ToString();
        }

        public string ObtenerInformacionDetalleLiquidacion_ArchivoCuentaPagar(List<DetalleLiquidacionParaArchivo> lista)
        {
            int consecutivo = 1;
            StringBuilder sbBody = new StringBuilder();

            foreach (var item in lista)
            {
                sbBody.Append(consecutivo);
                sbBody.Append("|");
                sbBody.Append(item.TipoDocumentoSoporteCodigo);
                sbBody.Append(Environment.NewLine);
                consecutivo++;
            }

            return sbBody.ToString();
        }

        #endregion Cuenta Por Pagar

        #region Obligacion
        public string ObtenerInformacionCabeceraLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> lista)
        {
            int consecutivo = 1;
            StringBuilder sbBody = new StringBuilder();

            foreach (var item in lista)
            {
                sbBody.Append(consecutivo);
                sbBody.Append("|");
                sbBody.Append(item.FechaActual);
                sbBody.Append("|");
                sbBody.Append(item.PCI);
                sbBody.Append("|");
                sbBody.Append(item.Crp);
                sbBody.Append("|");
                sbBody.Append(item.FechaActual);
                sbBody.Append("|");
                sbBody.Append(item.Dip);
                sbBody.Append("|");
                sbBody.Append(item.TipoCuentaXPagarCodigo);
                sbBody.Append("|");
                sbBody.Append(item.ValorIva.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append(item.TipoDocumentoSoporteCodigo);
                sbBody.Append("|");
                sbBody.Append(item.NumeroFactura);
                sbBody.Append("|");
                sbBody.Append(item.FechaActual);
                sbBody.Append("|");
                sbBody.Append(item.ConstanteExpedidor);
                sbBody.Append("|");
                sbBody.Append(item.NombreSupervisor);
                sbBody.Append("|");
                sbBody.Append(item.ConstanteCargo);
                sbBody.Append("|");
                sbBody.Append(item.TextoComprobanteContable);
                sbBody.Append("||");
                sbBody.Append(Environment.NewLine);
                consecutivo++;
            }
            return sbBody.ToString();
        }

        public string ObtenerInformacionItemsLiquidacion_ArchivoObligacion(List<ClavePresupuestalContableParaArchivo> listaTotal)
        {
            List<ClavePresupuestalContableParaArchivo> lista = null;
            int consecutivoCabecera = 1;
            int consecutivoInterno = 1;
            StringBuilder sbBody = new StringBuilder();

            var listaAgrupada = (from l in listaTotal
                                 group l by new { l.DetalleLiquidacionId }
                                 into grp
                                 select new
                                 {
                                     grp.Key.DetalleLiquidacionId
                                 });

            var listaLiquidacionIds = listaAgrupada.Select(s => s.DetalleLiquidacionId).ToList();

            foreach (var item in listaLiquidacionIds)
            {
                lista = listaTotal.Where(x => x.DetalleLiquidacionId == item).ToList();

                foreach (var itemInterno in lista)
                {
                    sbBody.Append(consecutivoCabecera);
                    sbBody.Append("|");
                    sbBody.Append(consecutivoInterno);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.Dependencia);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.RubroPresupuestalIdentificacion);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.RecursoPresupuestalCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.FuenteFinanciacionCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.SituacionFondoCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.ValorTotal.ToString().Replace(".", ","));
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.AtributoContableCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.TipoGastoCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.TipoOperacion);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.UsoContable);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.NumeroCuenta);
                    sbBody.Append(Environment.NewLine);

                    consecutivoInterno++;
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }

        public string ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(List<int> liquidacionIds, List<DeduccionDetalleLiquidacionParaArchivo> listaTotal)
        {
            List<DeduccionDetalleLiquidacionParaArchivo> lista = null;
            int consecutivoCabecera = 1;
            int consecutivoInterno = 1;
            StringBuilder sbBody = new StringBuilder();

            var listaAgrupada = (from l in listaTotal
                                 group l by new { l.DetalleLiquidacionId }
                                into grp
                                 select new
                                 {
                                     grp.Key.DetalleLiquidacionId
                                 });

            var listaLiquidacionIds = listaAgrupada.Select(s => s.DetalleLiquidacionId).ToList();

            foreach (var liquidacionId in liquidacionIds)
            {
                lista = listaTotal.Where(x => x.DetalleLiquidacionId == liquidacionId).ToList();

                if (lista != null && lista.Count > 0)
                {
                    foreach (var itemInterno in lista)
                    {
                        sbBody.Append(consecutivoCabecera);
                        sbBody.Append("|");
                        sbBody.Append(consecutivoInterno);
                        sbBody.Append("|");
                        sbBody.Append(itemInterno.DeduccionCodigo);
                        sbBody.Append("|");
                        sbBody.Append(itemInterno.TipoIdentificacion.ToString().PadLeft(2, '0'));
                        sbBody.Append("|");
                        sbBody.Append(itemInterno.NumeroIdentificacion);
                        sbBody.Append("|");
                        sbBody.Append((int)Math.Round(itemInterno.Base, 0, MidpointRounding.AwayFromZero)).ToString();
                        sbBody.Append("|");
                        sbBody.Append(itemInterno.Tarifa.ToString().Replace(".", ","));
                        sbBody.Append("|");
                        sbBody.Append((int)Math.Round(itemInterno.Valor, 0, MidpointRounding.AwayFromZero)).ToString();
                        sbBody.Append(Environment.NewLine);

                        consecutivoInterno++;
                    }
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }

        public string ObtenerInformacionUsosLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> listaUsos,
                                                                            List<ClavePresupuestalContableParaArchivo> listaRubros)
        {
            List<DetalleLiquidacionParaArchivo> listaUsoFiltrada = null;
            List<ClavePresupuestalContableParaArchivo> listaRubroFiltrada = null;
            int consecutivoCabecera = 1;
            int consecutivoInterno = 1;
            StringBuilder sbBody = new StringBuilder();

            var listaAgrupada = (from l in listaUsos
                                 group l by new { l.DetalleLiquidacionId }
                                into grp
                                 select new
                                 {
                                     grp.Key.DetalleLiquidacionId
                                 });

            var listaLiquidacionIds = listaAgrupada.Select(s => s.DetalleLiquidacionId).ToList();

            foreach (var item in listaLiquidacionIds)
            {
                listaUsoFiltrada = listaUsos.Where(x => x.DetalleLiquidacionId == item).ToList();
                listaRubroFiltrada = listaRubros.Where(x => x.DetalleLiquidacionId == item).ToList();

                foreach (var rubro in listaRubroFiltrada)
                {
                    var uso = listaUsoFiltrada
                                .Where(x => x.ClavePresupuestalContableId == rubro.ClavePresupuestalContableId).FirstOrDefault();

                    if (uso != null)
                    {
                        sbBody.Append(consecutivoCabecera);
                        sbBody.Append("|");
                        sbBody.Append(consecutivoInterno);
                        sbBody.Append("|");
                        sbBody.Append(uso.UsoPresupuestalCodigo);
                        sbBody.Append("|");
                        sbBody.Append(uso.ValorTotal.ToString().Replace(".", ","));
                        sbBody.Append(Environment.NewLine);
                        consecutivoInterno++;
                    }
                    consecutivoCabecera++;
                }
            }
            return sbBody.ToString();
        }


        public string ObtenerInformacionFacturaLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> listaTotal)
        {
            int consecutivoCabecera = 1;
            int consecutivoInterno = 1;
            StringBuilder sbBody = new StringBuilder();

            foreach (var item in listaTotal)
            {
                if (item.EsFacturaElectronica)
                {
                    sbBody.Append(consecutivoCabecera);
                    sbBody.Append("|");
                    sbBody.Append(consecutivoInterno);
                    sbBody.Append("|");
                    sbBody.Append(item.NumeroFactura);
                    sbBody.Append(Environment.NewLine);

                    consecutivoInterno++;
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }


        #endregion Obligacion

        #region Registrar archivo

        public ArchivoDetalleLiquidacion RegistrarArchivoDetalleLiquidacion(int usuarioId, int pciId, List<int> listIds,
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

        public string ObtenerNombreArchivo(DateTime fecha, int consecutivo, int tipoDocumentoArchivo, int tipoArchivo)
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

        public void RegistrarDetalleArchivoLiquidacion(int archivoId, List<int> listIds)
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


        #endregion Registrar archivo
    }
}
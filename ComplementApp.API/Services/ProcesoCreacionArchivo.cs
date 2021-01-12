using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Interfaces;

namespace ComplementApp.API.Services
{
    public class ProcesoCreacionArchivo : IProcesoCreacionArchivo
    {
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
                sbBody.Append("0" + item.TipoIdentificacion);
                sbBody.Append("|");
                sbBody.Append(item.NumeroIdentificacion);
                sbBody.Append("|");
                sbBody.Append(item.Crp);
                sbBody.Append("|");
                sbBody.Append(item.TipoCuentaPagar);
                sbBody.Append("|");
                sbBody.Append(item.TotalACancelar.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append("0");
                sbBody.Append("|");
                sbBody.Append(item.ValorIva.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append(item.TextoComprobanteContable);
                sbBody.Append("|");
                sbBody.Append(item.TipoDocumentoSoporte);
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
                sbBody.Append(item.TipoDocumentoSoporte);
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
                sbBody.Append(item.TipoCuentaPagarCodigo);
                sbBody.Append("|");
                sbBody.Append(item.ValorIva.ToString().Replace(".", ","));
                sbBody.Append("|");
                sbBody.Append(item.TipoDocumentoSoporte);
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
                sbBody.Append("|||");
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
                    sbBody.Append("00" + itemInterno.Dependencia);
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
                    sbBody.Append("||");
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


        public string ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(List<DeduccionDetalleLiquidacionParaArchivo> listaTotal)
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

            foreach (var item in listaLiquidacionIds)
            {
                lista = listaTotal.Where(x => x.DetalleLiquidacionId == item).ToList();

                foreach (var itemInterno in lista)
                {
                    sbBody.Append(consecutivoCabecera);
                    sbBody.Append("|");
                    sbBody.Append(consecutivoInterno);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.DeduccionCodigo);
                    sbBody.Append("|");
                    sbBody.Append("0" + itemInterno.TipoIdentificacion);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.NumeroIdentificacion);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.Base.ToString().Replace(".", ","));
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.Tarifa.ToString().Replace(".", ","));
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.Valor.ToString().Replace(".", ","));
                    sbBody.Append(Environment.NewLine);

                    consecutivoInterno++;
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }

        public string ObtenerInformacionUsosLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> listaTotal)
        {
            List<DetalleLiquidacionParaArchivo> lista = null;
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
                    sbBody.Append(itemInterno.UsoPresupuestalCodigo);
                    sbBody.Append("|");
                    sbBody.Append(itemInterno.ValorTotal.ToString().Replace(".", ","));
                    sbBody.Append(Environment.NewLine);
                    consecutivoInterno++;
                }
                consecutivoCabecera++;
            }
            return sbBody.ToString();
        }


        #endregion Obligacion
    }
}
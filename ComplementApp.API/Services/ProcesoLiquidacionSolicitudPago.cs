using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;

namespace ComplementApp.API.Services
{
    public class ProcesoLiquidacionSolicitudPago : IProcesoLiquidacionSolicitudPago
    {
        private readonly IListaRepository _repoLista;
        private readonly IPlanPagoRepository _planPagoRepository;
        private readonly ITerceroRepository _terceroRepository;
        private readonly IGeneralInterface _generalInterface;

        #region Constantes

        const string codigoRenta = "CodigoRenta";
        const string codigoIva = "CodigoIva";
        const string valorUVT = "ValorUVT";
        const string salarioMinimo = "SalarioMinimo";
        const string codigoPensionVoluntaria = "CodigoPensionVoluntaria";
        const string codigoAFC = "CodigoAFC";

        #endregion Constantes

        public ProcesoLiquidacionSolicitudPago(IListaRepository listaRepository, IPlanPagoRepository planPagoRepository,
        ITerceroRepository terceroRepository, IGeneralInterface generalInterface)
        {
            this._repoLista = listaRepository;
            this._planPagoRepository = planPagoRepository;
            this._terceroRepository = terceroRepository;
            this._generalInterface = generalInterface;
        }
        public async Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoSolicitudPago(int planPagoId,
                                                                                        decimal valorBaseCotizacion,
                                                                                        int? actividadEconomicaId)
        {
            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();
            try
            {
                var planPagoDto = await _planPagoRepository.ObtenerDetallePlanPago(planPagoId);

                IEnumerable<ParametroGeneral> parametroGenerales = await _repoLista.ObtenerParametrosGenerales();
                var parametros = parametroGenerales.ToList();

                ParametroLiquidacionTercero parametroLiquidacion = await _terceroRepository.ObtenerParametroLiquidacionXTercero(planPagoDto.TerceroId);

                IEnumerable<ValorSeleccion> listaAdminPila = await _repoLista.ObtenerListaXTipo(TipoLista.TipoAdminPila);
                var listaTipoAdminPila = listaAdminPila.ToList();

                if (parametroGenerales != null && parametroLiquidacion != null && listaTipoAdminPila != null)
                {
                    if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ContratoPrestacionServicio)
                    {
                        formato = ObtenerFormatoCausacion_ContratoPrestacionServicio(planPagoDto, parametroLiquidacion, parametros, listaTipoAdminPila, valorBaseCotizacion);
                    }
                    formato.PlanPagoId = planPagoId;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return formato;
        }

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ContratoPrestacionServicio(DetallePlanPagoDto planPago,
                                        ParametroLiquidacionTercero parametroLiquidacion,
                                        List<ParametroGeneral> parametroGenerales,
                                        List<ValorSeleccion> listaAdminPila,
                                        decimal valorBaseCotizacion)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual30 = new FormatoCausacionyLiquidacionPagos();

            #endregion variables 

            #region Calcular valores y obtener formato

            formatoIgual30 = CalcularValoresFormatoContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales, valorBaseCotizacion);
            formato = formatoIgual30;

            #endregion Calcular valores y obtener formato

            var adminPila = listaAdminPila.Where(x => x.Id == parametroLiquidacion.TipoAdminPilaId).FirstOrDefault();
            if (adminPila != null)
            {
                formato.TipoAdminPila = adminPila.Nombre;
            }


            return formato;
        }



        private FormatoCausacionyLiquidacionPagos CalcularValoresFormatoContratoPrestacionServicio(
                                         DetallePlanPagoDto planPago,
                                         ParametroLiquidacionTercero parametroLiquidacion,
                                         List<ParametroGeneral> parametroGenerales,
                                         decimal valorBaseCotizacion)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            decimal valorUvt = 0, valorSMLV = 0;

            decimal PLtarifaIva = 0, PLbaseAporteSalud = 0, PLaporteSalud = 0,
            PLaportePension = 0, PLriesgoLaboral = 0, PLfondoSolidaridad = 0;

            decimal C1honorario = 0, C2honorarioUvt = 0,
            C8aporteASalud = 0, C9aporteAPension = 0, C10aporteRiesgoLaboral = 0,
            C7baseAporteSalud = 0, C11fondoSolidaridad;

            int C2honorarioUvtFinal = 0;

            decimal cuatroSMLV = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLtarifaIva = parametroLiquidacion.TarifaIva;
            PLbaseAporteSalud = parametroLiquidacion.BaseAporteSalud;
            PLaporteSalud = parametroLiquidacion.AporteSalud;
            PLaportePension = parametroLiquidacion.AportePension;
            PLriesgoLaboral = parametroLiquidacion.RiesgoLaboral;
            PLfondoSolidaridad = parametroLiquidacion.FondoSolidaridad;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C7baseAporteSalud = valorBaseCotizacion;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = _generalInterface.ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                cuatroSMLV = 4 * valorSMLV;
            }

            #region Fondo de solidaridad

            if (C7baseAporteSalud > cuatroSMLV)
            {
                C11fondoSolidaridad = C7baseAporteSalud * (PLfondoSolidaridad);
                C11fondoSolidaridad = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C11fondoSolidaridad);
            }
            else
            {
                C11fondoSolidaridad = 0;
            }

            #endregion Fondo de solidaridad

            #region Setear valores a formato

            formato.AporteSalud = C8aporteASalud;
            formato.AportePension = C9aporteAPension;
            formato.RiesgoLaboral = C10aporteRiesgoLaboral;
            formato.FondoSolidaridad = C11fondoSolidaridad;

            #endregion Setear valores a formato

            return formato;
        }

        private string ObtenerValorDeParametroGeneral(List<ParametroGeneral> parametroGenerales, string nombre)
        {
            string valor = string.Empty;
            var item = parametroGenerales.FirstOrDefault(x => x.Nombre.ToUpper() == nombre.ToUpper());

            if (item != null)
                valor = item.Valor;

            return valor;
        }

    }
}
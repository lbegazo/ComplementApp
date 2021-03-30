using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text;
using EFCore.BulkExtensions;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using ComplementApp.API.Data;

namespace ComplementApp.API.Services
{
    public class ProcesoLiquidacionPlanPago : IProcesoLiquidacionPlanPago
    {
        #region Propiedades        
        private List<CriterioCalculoReteFuente> listaCriterioCalculoReteFuente = null;
        private List<ParametroGeneral> listaParametrosGenerales = null;
        private List<int> terceroConMasUnaActividadEconomica = new List<int>();

        #endregion Propiedades

        #region Constantes

        const string codigoRenta = "CodigoRenta";
        const string codigoIva = "CodigoIva";
        const string valorUVT = "ValorUVT";
        const string salarioMinimo = "SalarioMinimo";
        const string codigoPensionVoluntaria = "CodigoPensionVoluntaria";
        const string codigoAFC = "CodigoAFC";

        #endregion Constantes

        #region Dependency Injection
        private readonly IDetalleLiquidacionRepository _repo;
        private readonly IPlanPagoRepository _planPagoRepository;
        private readonly IListaRepository _repoLista;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly ITerceroRepository _terceroRepository;
        private readonly ISolicitudPagoRepository _solicitudPagoRepository;
        private readonly DataContext _dataContext;

        #endregion Dependency Injection

        public ProcesoLiquidacionPlanPago(IPlanPagoRepository planPagoRepository,
                                            IDetalleLiquidacionRepository repo,
                                            IListaRepository listaRepository,
                                            IMapper mapper,
                                            IGeneralInterface generalInterface,
                                            ITerceroRepository terceroRepository,
                                            ISolicitudPagoRepository solicitudPagoRepository,
                                            DataContext dataContext)
        {
            this._planPagoRepository = planPagoRepository;
            this._repo = repo;
            this._repoLista = listaRepository;
            this._mapper = mapper;
            this._generalInterface = generalInterface;
            this._terceroRepository = terceroRepository;
            this._solicitudPagoRepository = solicitudPagoRepository;
            _dataContext = dataContext;
        }

        #region Liquidación Normal
        public async Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacionyLiquidacionPago(int planPagoId,
                                                                                                     int pciId,
                                                                                                     decimal valorBaseGravable,
                                                                                                     int? actividadEconomicaId)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            PlanPagoDto planPagoDto2 = new PlanPagoDto();

            try
            {
                var planPagoDto = await _planPagoRepository.ObtenerDetallePlanPago(planPagoId);
                planPagoDto2.ValorFacturado = planPagoDto.ValorFacturado.Value;

                IEnumerable<ParametroGeneral> parametroGenerales = await _repoLista.ObtenerParametrosGenerales();
                var parametros = parametroGenerales.ToList();

                ParametroLiquidacionTercero parametroLiquidacion = await _terceroRepository.ObtenerParametroLiquidacionXTercero(planPagoDto.TerceroId, pciId);

                //Podría ser nulo la solicitud de pago
                FormatoSolicitudPago solicitudPago = await _solicitudPagoRepository.ObtenerSolicitudPagoXPlanPagoId(planPagoId);
                actividadEconomicaId = solicitudPago.ActividadEconomicaId;

                ICollection<Deduccion> listaDeducciones = await _terceroRepository.ObtenerDeduccionesXTercero(planPagoDto.TerceroId, pciId, actividadEconomicaId);
                var listaDeduccionesDto = _mapper.Map<ICollection<DeduccionDto>>(listaDeducciones);

                ICollection<CriterioCalculoReteFuente> listaCriterioReteFuente = await _repoLista.ObtenerListaCriterioCalculoReteFuente();

                if (parametroGenerales != null && parametroLiquidacion != null && listaCriterioReteFuente != null)
                {
                    if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ContratoPrestacionServicio)
                    {
                        if (!parametroLiquidacion.Subcontrata)
                        {
                            if (!planPagoDto.Viaticos)
                            {
                                formato = await ObtenerFormatoCausacion_ContratoPrestacionServicio(planPagoDto, parametroLiquidacion,
                                                                            parametros, listaCriterioReteFuente.ToList(),
                                                                            listaDeduccionesDto.ToList());
                            }
                            else
                            {
                                //Contratistas con viaticos liquida de otra manera
                                formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto2, parametroLiquidacion,
                                                                                            parametros, listaCriterioReteFuente.ToList());
                            }
                        }
                        else
                        {
                            formato = ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(planPagoDto2, parametroLiquidacion,
                                                                                                    parametros, listaCriterioReteFuente.ToList(),
                                                                                                    listaDeduccionesDto.ToList());
                        }
                    }
                    else if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ProveedorConDescuento)
                    {
                        if (parametroLiquidacion.TipoPago == (int)TipoPago.Fijo)
                        {
                            formato = ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(planPagoDto2, parametroLiquidacion,
                                                                                                    parametros, listaCriterioReteFuente.ToList(),
                                                                                                    listaDeduccionesDto.ToList());
                        }
                        else
                        {
                            valorBaseGravable = solicitudPago.ValorBaseGravableRenta;

                            formato = ObtenerFormatoCausacion_ProveedoresConDeduccionVariable(planPagoDto2, solicitudPago,
                                                                        parametroLiquidacion,
                                                                        parametros, listaCriterioReteFuente.ToList(),
                                                                        listaDeduccionesDto.ToList(), valorBaseGravable);
                        }

                    }
                    else if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ProveedorSinDescuento)
                    {
                        formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto2, parametroLiquidacion,
                                                                                  parametros, listaCriterioReteFuente.ToList());
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

        #region Contrato Prestación de Servicio

        private async Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacion_ContratoPrestacionServicio(DetallePlanPagoDto planPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            List<CriterioCalculoReteFuente> listaCriterioReteFuente,
                                            List<DeduccionDto> listaDeducciones)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual30 = new FormatoCausacionyLiquidacionPagos();
            CriterioCalculoReteFuente criterioReteFuente = null;

            decimal C30ValorIva = 0, PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0,
            baseGravableFinal = 0, valorUvt, baseGravableUvtCalculada = 0;
            int C32NumeroDiaLaborados = 0, baseGravableUvtFinal = 0;
            decimal factorCalculo = 0;

            decimal valor = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PL17HonorarioSinIva = ObtenerHonorarioSinIva(parametroLiquidacion, planPago.ValorFacturado.Value);

            var tarifaIva = parametroLiquidacion.TarifaIva;
            var GmfAfc = parametroLiquidacion.GmfAfc;
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);

            var parametrosCodigoRenta = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoRenta);
            var parametrosCodigoIva = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoIva);
            var parametrosCodigoPensionVoluntaria = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoPensionVoluntaria);
            var parametrosCodigoAFC = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoAFC);

            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales           

            #region Numero de dias laborados

            if (PL17HonorarioSinIva > 0)
            {
                valor = (((planPago.ValorFacturado.Value / (1 + tarifaIva)) * 30) / PL17HonorarioSinIvaParametro);
                C32NumeroDiaLaborados = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                factorCalculo = ((decimal)C32NumeroDiaLaborados / (decimal)30);
            }

            #endregion Numero de dias laborados

            #region Calcular valores y obtener formato

            formatoIgual = await CalcularValoresFormatoContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: 30, factorCalculo);
            formatoIgual30 = await CalcularValoresBaseGravableParaContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales);

            formato = formatoIgual;
            baseGravableUvtCalculada = formatoIgual30.BaseGravableUvtCalculada;
            baseGravableFinal = formatoIgual30.BaseGravableRenta;
            baseGravableUvtFinal = formatoIgual30.BaseGravableUvt;

            formato.BaseGravableUvtCalculada = formatoIgual30.BaseGravableUvtCalculada;
            formato.BaseGravableRenta = formatoIgual30.BaseGravableRenta;
            formato.BaseGravableUvt = formatoIgual30.BaseGravableUvt;

            #endregion Calcular valores y obtener formato

            #region Obtener Lista de deducciones

            C30ValorIva = formato.ValorIva;

            decimal valorMinimoRango = 0;
            decimal factorIncremento = 0;
            decimal tarifaCalculo = 0;

            criterioReteFuente = ObtenerCriterioCalculoRendimiento(listaCriterioReteFuente, baseGravableUvtCalculada);

            valorMinimoRango = criterioReteFuente.Desde;
            factorIncremento = criterioReteFuente.Factor;
            tarifaCalculo = criterioReteFuente.Tarifa;

            if (listaDeducciones != null && listaDeducciones.Count > 0)
            {
                foreach (var deduccion in listaDeducciones)
                {
                    if (DeduccionEsParametroGeneral(parametrosCodigoRenta, deduccion.Codigo))
                    {
                        deduccion.Base = baseGravableFinal;
                        var valorRentaCalculado = ((((tarifaCalculo / 100) * (baseGravableUvtCalculada - valorMinimoRango))
                                                        + factorIncremento) * valorUvt) * factorCalculo;
                        deduccion.Valor = this._generalInterface.ObtenerValorRedondeadoCPS(valorRentaCalculado);

                        if (deduccion.Base > 0)
                        {
                            deduccion.Tarifa = deduccion.Valor / deduccion.Base;
                        }
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoIva, deduccion.Codigo))
                    {
                        deduccion.Base = C30ValorIva;
                        valor = deduccion.Tarifa * C30ValorIva;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);

                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoAFC, deduccion.Codigo))
                    {
                        deduccion.Base = formato.Afc;
                        valor = deduccion.Tarifa * deduccion.Base;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoPensionVoluntaria, deduccion.Codigo))
                    {
                        deduccion.Base = formato.PensionVoluntaria;
                        valor = deduccion.Tarifa * deduccion.Base;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if ((deduccion.TipoBaseDeduccionId != (int)TipoBaseDeducciones.OTRAS))
                    {
                        deduccion.Base = formato.SubTotal1;
                        valor = deduccion.Tarifa * formato.SubTotal1;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }

            #region Deducción Otras

            var deduccionOtras = listaDeducciones
                                    .Where(x => x.TipoBaseDeduccionId == (int)TipoBaseDeducciones.OTRAS)
                                    .FirstOrDefault();

            if (deduccionOtras != null)
            {
                var valorGmf = obtenerSumatoriaValorGMf(listaDeducciones, parametrosCodigoAFC, GmfAfc);
                deduccionOtras.Base = valorGmf;
                valor = deduccionOtras.Tarifa * valorGmf;
                valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                deduccionOtras.Valor = valor;

            }

            #endregion Deducción Otras

            formato.Deducciones = new List<DeduccionDto>();
            formato.Deducciones = listaDeducciones;

            #endregion Obtener lista de deducciones          

            formato.TotalRetenciones = obtenerSumatoriaRetenciones(listaDeducciones);
            formato.TotalAGirar = formato.ValorTotal - formato.TotalRetenciones;

            return formato;
        }

        private decimal ObtenerHonorarioSinIva(ParametroLiquidacionTercero parametroLiquidacion, decimal valorFacturado)
        {
            decimal honorarioReal = 0;

            if (parametroLiquidacion.TarifaIva == 0)
            {
                honorarioReal = valorFacturado;
            }
            else
            {
                honorarioReal = valorFacturado / (1 + parametroLiquidacion.TarifaIva);
            }
            return honorarioReal;
        }

        private decimal obtenerSumatoriaValorGMf(List<DeduccionDto> listaDeducciones)
        {
            decimal valor = 0;

            valor = listaDeducciones
                    .Where(x => x.Gmf == true)
                    .Sum(x => x.Valor);


            return valor;
        }

        private decimal obtenerSumatoriaValorGMf(List<DeduccionDto> listaDeducciones, List<ParametroGeneral> parametrosCodigoAFC, bool GmfAfc)
        {
            decimal valor = 0;

            foreach (var deduccion in listaDeducciones)
            {
                //Del tipo AFC
                if (DeduccionEsParametroGeneral(parametrosCodigoAFC, deduccion.Codigo))
                {
                    if (GmfAfc)
                    {
                        valor = valor + deduccion.Valor;
                    }
                }
                else
                {
                    if (deduccion.Gmf)
                    {
                        valor = valor + deduccion.Valor;
                    }
                }
            }
            return valor;
        }

        private decimal obtenerSumatoriaRetenciones(List<DeduccionDto> listaDeducciones)
        {
            decimal valor = 0;

            valor = listaDeducciones
                    .Sum(x => x.Valor);

            return valor;
        }

        private async Task<FormatoCausacionyLiquidacionPagos> CalcularValoresFormatoContratoPrestacionServicio(
                                            DetallePlanPagoDto planPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            int numeroDiasLaborados,
                                            decimal factorCalculo)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            DateTime? fechaInicio = null, fechaFinal = null;

            decimal valorUvt = 0, valorSMLV = 0;
            decimal valorDescuentoDependiente = 0.10m;
            decimal valorRentaExenta = 0.25m;
            decimal valorLimiteRentaExenta = 0.4m;
            decimal valorDiferencialRenta = 0.3m;

            decimal PLtarifaIva = 0, PLbaseAporteSalud = 0, PLaporteSalud = 0,
            PLaportePension = 0, PLriesgoLaboral = 0, PLfondoSolidaridad = 0,
            PL14Afc = 0, PL16MedicinaPrepagada = 0,
            PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0, PL17DescuentoDependiente = 0,
            PL18InteresVivienda = 0, PL24PensionVoluntaria = 0, viaticosPagados = 0;

            decimal C1honorario = 0, C2honorarioUvt = 0, C3valorIva = 0,
            C8aporteASalud = 0, C9aporteAPension = 0, C10aporteRiesgoLaboral = 0,
            C7baseAporteSalud = 0, C11fondoSolidaridad, C12subTotal1 = 0,
            C15subTotal2 = 0, C20subTotal3 = 0, C17DescuentoDependiente = 0,
            C19TotalDeducciones = 0, C21RentaExenta = 0, C22LimiteRentaExenta = 0,
            C23TotalRentaExenta = 0, C24DiferencialRenta = 0,
            C25BaseGravableRenta = 0, C26BaseGravableRentaUvt = 0;

            int C26BaseGravableRentaUvtFinal = 0, C2honorarioUvtFinal = 0;

            decimal cuatroSMLV = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLtarifaIva = parametroLiquidacion.TarifaIva;
            PLbaseAporteSalud = parametroLiquidacion.BaseAporteSalud;
            PLaporteSalud = parametroLiquidacion.AporteSalud;
            PLaportePension = parametroLiquidacion.AportePension;
            PLriesgoLaboral = parametroLiquidacion.RiesgoLaboral;
            PLfondoSolidaridad = parametroLiquidacion.FondoSolidaridad;
            PL14Afc = parametroLiquidacion.Afc;
            PL16MedicinaPrepagada = parametroLiquidacion.MedicinaPrepagada;
            PL17HonorarioSinIva = ObtenerHonorarioSinIva(parametroLiquidacion, planPago.ValorFacturado.Value);
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;

            if (factorCalculo == 1)
            {
                PL17HonorarioSinIva = PL17HonorarioSinIvaParametro;
            }


            PL17DescuentoDependiente = parametroLiquidacion.Dependiente;
            PL18InteresVivienda = parametroLiquidacion.InteresVivienda;
            PL24PensionVoluntaria = parametroLiquidacion.PensionVoluntaria;

            if (parametroLiquidacion.FechaInicioDescuentoInteresVivienda.HasValue)
                fechaInicio = parametroLiquidacion.FechaInicioDescuentoInteresVivienda.Value;

            if (parametroLiquidacion.FechaFinalDescuentoInteresVivienda.HasValue)
                fechaFinal = parametroLiquidacion.FechaFinalDescuentoInteresVivienda.Value;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            #region Viaticos y Aporte Salud Anterior

            var listaDetalleLiquidacionViaticos = await _repo.ObtenerListaDetalleLiquidacionViaticosAnterior(planPago.TerceroId, parametroLiquidacion.PciId.Value);

            if (listaDetalleLiquidacionViaticos != null && listaDetalleLiquidacionViaticos.Count > 0)
            {
                viaticosPagados = ObtenerValorViaticosAnteriores(listaDetalleLiquidacionViaticos.ToList());
            }

            var detalleLiquidacionAnterior = await _repo.ObtenerDetalleLiquidacionAnterior(planPago.TerceroId, parametroLiquidacion.PciId.Value);

            if (detalleLiquidacionAnterior != null)
            {
                formato.NumeroMesSaludAnterior = detalleLiquidacionAnterior.MesSaludAnterior;
                formato.MesSaludAnterior = (detalleLiquidacionAnterior.MesSaludAnterior > 0 && detalleLiquidacionAnterior.MesSaludAnterior < 13) ?
                                            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(detalleLiquidacionAnterior.MesSaludAnterior).ToUpper() :
                                            string.Empty;
            }

            #endregion Viáticos y Aporte Salud Anterior

            #region Honorario 

            C1honorario = PL17HonorarioSinIva + viaticosPagados;

            #endregion Honorario 

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C3valorIva = C1honorario * PLtarifaIva;
            C7baseAporteSalud = (C1honorario - viaticosPagados) * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                //Se cambia de forma temporal, debe ser 4
                cuatroSMLV = 2 * valorSMLV;
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

            C12subTotal1 = C1honorario - C8aporteASalud - C9aporteAPension - C10aporteRiesgoLaboral - C11fondoSolidaridad;
            C15subTotal2 = C12subTotal1 - PL24PensionVoluntaria - PL14Afc;

            #region Descuento Dependiente

            if (PL17HonorarioSinIvaParametro * valorDescuentoDependiente > valorUvt * 32)
            {
                if (PL17DescuentoDependiente > 0)
                {
                    C17DescuentoDependiente = valorUvt * 32;
                }
            }
            else
            {
                C17DescuentoDependiente = PL17HonorarioSinIvaParametro * PL17DescuentoDependiente;
            }

            #endregion Descuento Dependiente

            #region Interes de vivienda

            if (fechaInicio != null && fechaFinal != null)
            {
                if (!FechaActualEntreFechasVigencia(fechaInicio.Value, fechaFinal.Value))
                {
                    PL18InteresVivienda = 0;
                }
            }

            #endregion Interes de vivienda

            #region Total Deducciones

            decimal CT16MedicinaPrepagadaDeduccion = 0, CT17DescuentoDependienteDeduccion = 0,
            CT18InteresViviendaDeduccion = 0;

            //Deduccion Medicina Prepagada
            if (PL16MedicinaPrepagada > valorUvt * 16)
            {
                CT16MedicinaPrepagadaDeduccion = valorUvt * 16;
            }
            else
            {
                CT16MedicinaPrepagadaDeduccion = PL16MedicinaPrepagada;
            }

            //Deducción Descuento Dependiente
            if (PL17DescuentoDependiente > valorUvt * 32)
            {
                CT17DescuentoDependienteDeduccion = valorUvt * 32;
            }
            else
            {
                CT17DescuentoDependienteDeduccion = C17DescuentoDependiente;
            }

            //Deducción Interes Vivienda
            if (PL18InteresVivienda > valorUvt * 100)
            {
                CT18InteresViviendaDeduccion = valorUvt * 100;
            }
            else
            {
                CT18InteresViviendaDeduccion = PL18InteresVivienda;
            }

            C19TotalDeducciones = CT16MedicinaPrepagadaDeduccion + CT17DescuentoDependienteDeduccion + CT18InteresViviendaDeduccion;

            #endregion Total Deducciones

            C20subTotal3 = C15subTotal2 - PL16MedicinaPrepagada - C17DescuentoDependiente - PL18InteresVivienda;

            #region Renta exenta

            if (C20subTotal3 * valorRentaExenta > 240 * valorUvt)
            {
                C21RentaExenta = 240 * valorUvt;
            }
            else
            {
                C21RentaExenta = C20subTotal3 * valorRentaExenta;
            }

            C22LimiteRentaExenta = C12subTotal1 * valorLimiteRentaExenta;
            C23TotalRentaExenta = PL14Afc + PL16MedicinaPrepagada + C17DescuentoDependiente + PL18InteresVivienda + C21RentaExenta;

            #endregion Renta exenta

            #region Diferencial Renta 

            decimal CT24DiferenciaRenta = 0;

            if ((PL24PensionVoluntaria + PL14Afc) > (C1honorario * valorDiferencialRenta))
            {
                CT24DiferenciaRenta = C1honorario * valorDiferencialRenta;
            }
            else
            {
                CT24DiferenciaRenta = PL24PensionVoluntaria + PL14Afc;
            }

            C24DiferencialRenta = CT24DiferenciaRenta + C19TotalDeducciones + C21RentaExenta;

            if (C24DiferencialRenta > C22LimiteRentaExenta)
            {
                C24DiferencialRenta = C22LimiteRentaExenta - C24DiferencialRenta;
            }
            else
            {
                C24DiferencialRenta = 0;
            }

            #endregion Diferencial Renta 

            #region Base Gravable

            C25BaseGravableRenta = (C20subTotal3 - C21RentaExenta - C24DiferencialRenta);

            C26BaseGravableRentaUvt = (C25BaseGravableRenta / valorUvt) / factorCalculo;
            C26BaseGravableRentaUvtFinal = (int)Math.Round(C26BaseGravableRentaUvt, 0, MidpointRounding.AwayFromZero);

            #endregion Base Gravable

            #region Setear valores a formato

            formato.Honorario = C1honorario;
            formato.HonorarioUvt = C2honorarioUvtFinal;
            formato.ValorIva = C3valorIva;
            formato.ValorTotal = planPago.ValorFacturado.Value;
            formato.BaseSalud = C7baseAporteSalud;
            formato.AporteSalud = C8aporteASalud;
            formato.AportePension = C9aporteAPension;
            formato.RiesgoLaboral = C10aporteRiesgoLaboral;
            formato.FondoSolidaridad = C11fondoSolidaridad;
            formato.SubTotal1 = C12subTotal1;
            formato.Afc = PL14Afc;
            formato.PensionVoluntaria = PL24PensionVoluntaria;
            formato.SubTotal2 = C15subTotal2;
            formato.MedicinaPrepagada = PL16MedicinaPrepagada;
            formato.Dependientes = C17DescuentoDependiente;
            formato.SubTotal3 = C20subTotal3;
            formato.RentaExenta = C21RentaExenta;
            formato.LimiteRentaExenta = C22LimiteRentaExenta;
            formato.TotalRentaExenta = C23TotalRentaExenta;
            formato.InteresVivienda = CT18InteresViviendaDeduccion;
            formato.TotalDeducciones = C19TotalDeducciones;
            formato.DiferencialRenta = C24DiferencialRenta;
            formato.BaseGravableRenta = C25BaseGravableRenta;
            formato.BaseGravableUvt = C26BaseGravableRentaUvtFinal;
            formato.BaseGravableUvtCalculada = C26BaseGravableRentaUvt;
            formato.ViaticosPagados = viaticosPagados;

            #endregion Setear valores a formato

            return formato;
        }

        private async Task<FormatoCausacionyLiquidacionPagos> CalcularValoresBaseGravableParaContratoPrestacionServicio(
                                                 DetallePlanPagoDto planPago,
                                                 ParametroLiquidacionTercero parametroLiquidacion,
                                                 List<ParametroGeneral> parametroGenerales)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            DateTime? fechaInicio = null, fechaFinal = null;

            decimal valorUvt = 0, valorSMLV = 0;
            decimal valorDescuentoDependiente = 0.10m;
            decimal valorRentaExenta = 0.25m;
            decimal valorLimiteRentaExenta = 0.4m;

            decimal PLtarifaIva = 0, PLbaseAporteSalud = 0, PLaporteSalud = 0,
            PLaportePension = 0, PLriesgoLaboral = 0, PLfondoSolidaridad = 0,
            PL14Afc = 0, PL16MedicinaPrepagada = 0,
            PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0, PL17DescuentoDependiente = 0,
            PL18InteresVivienda = 0, PL24PensionVoluntaria = 0, viaticosPagados = 0;

            decimal C1honorario = 0, C2honorarioUvt = 0, C3valorIva = 0,
            C8aporteASalud = 0, C9aporteAPension = 0, C10aporteRiesgoLaboral = 0,
            C7baseAporteSalud = 0, C11fondoSolidaridad, C12subTotal1 = 0,
            C15subTotal2 = 0, C20subTotal3 = 0, C17DescuentoDependiente = 0,
            C19TotalDeducciones = 0, C21RentaExenta = 0, C22LimiteRentaExenta = 0,
            C23TotalRentaExenta = 0,
            C25BaseGravableRenta = 0, C26BaseGravableRentaUvt = 0;

            int C26BaseGravableRentaUvtFinal = 0, C2honorarioUvtFinal = 0;

            decimal cuatroSMLV = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLtarifaIva = parametroLiquidacion.TarifaIva;
            PLbaseAporteSalud = parametroLiquidacion.BaseAporteSalud;
            PLaporteSalud = parametroLiquidacion.AporteSalud;
            PLaportePension = parametroLiquidacion.AportePension;
            PLriesgoLaboral = parametroLiquidacion.RiesgoLaboral;
            PLfondoSolidaridad = parametroLiquidacion.FondoSolidaridad;
            PL14Afc = parametroLiquidacion.Afc;
            PL16MedicinaPrepagada = parametroLiquidacion.MedicinaPrepagada;
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;
            PL17HonorarioSinIva = PL17HonorarioSinIvaParametro;

            PL17DescuentoDependiente = parametroLiquidacion.Dependiente;
            PL18InteresVivienda = parametroLiquidacion.InteresVivienda;
            PL24PensionVoluntaria = parametroLiquidacion.PensionVoluntaria;

            if (parametroLiquidacion.FechaInicioDescuentoInteresVivienda.HasValue)
                fechaInicio = parametroLiquidacion.FechaInicioDescuentoInteresVivienda.Value;

            if (parametroLiquidacion.FechaFinalDescuentoInteresVivienda.HasValue)
                fechaFinal = parametroLiquidacion.FechaFinalDescuentoInteresVivienda.Value;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            #region Viaticos y Aporte Salud Anterior

            var listaDetalleLiquidacionViaticos = await _repo.ObtenerListaDetalleLiquidacionViaticosAnterior(planPago.TerceroId, parametroLiquidacion.PciId.Value);

            if (listaDetalleLiquidacionViaticos != null && listaDetalleLiquidacionViaticos.Count > 0)
            {
                viaticosPagados = ObtenerValorViaticosAnteriores(listaDetalleLiquidacionViaticos.ToList());
            }

            #endregion Viáticos y Aporte Salud Anterior

            #region Honorario 

            C1honorario = PL17HonorarioSinIva + viaticosPagados;

            #endregion Honorario 

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C3valorIva = C1honorario * PLtarifaIva;
            C7baseAporteSalud = (C1honorario - viaticosPagados) * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                //Se cambia de forma temporal, debe ser 4
                cuatroSMLV = 2 * valorSMLV;
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

            C12subTotal1 = C1honorario - C8aporteASalud - C9aporteAPension - C10aporteRiesgoLaboral - C11fondoSolidaridad;
            C15subTotal2 = C12subTotal1 - PL24PensionVoluntaria - PL14Afc;

            #region Descuento Dependiente

            if (PL17HonorarioSinIvaParametro * valorDescuentoDependiente > valorUvt * 32)
            {
                if (PL17DescuentoDependiente > 0)
                {
                    C17DescuentoDependiente = valorUvt * 32;
                }
            }
            else
            {
                C17DescuentoDependiente = PL17HonorarioSinIvaParametro * PL17DescuentoDependiente;
            }

            #endregion Descuento Dependiente

            #region Interes de vivienda

            if (fechaInicio != null && fechaFinal != null)
            {
                if (!FechaActualEntreFechasVigencia(fechaInicio.Value, fechaFinal.Value))
                {
                    PL18InteresVivienda = 0;
                }
            }

            #endregion Interes de vivienda

            #region Total Deducciones

            decimal CT16MedicinaPrepagadaDeduccion = 0, CT17DescuentoDependienteDeduccion = 0,
            CT18InteresViviendaDeduccion = 0;

            //Deduccion Medicina Prepagada
            if (PL16MedicinaPrepagada > valorUvt * 16)
            {
                CT16MedicinaPrepagadaDeduccion = valorUvt * 16;
            }
            else
            {
                CT16MedicinaPrepagadaDeduccion = PL16MedicinaPrepagada;
            }

            //Deducción Descuento Dependiente
            if (PL17DescuentoDependiente > valorUvt * 32)
            {
                CT17DescuentoDependienteDeduccion = valorUvt * 32;
            }
            else
            {
                CT17DescuentoDependienteDeduccion = C17DescuentoDependiente;
            }

            //Deducción Interes Vivienda
            if (PL18InteresVivienda > valorUvt * 100)
            {
                CT18InteresViviendaDeduccion = valorUvt * 100;
            }
            else
            {
                CT18InteresViviendaDeduccion = PL18InteresVivienda;
            }

            C19TotalDeducciones = CT16MedicinaPrepagadaDeduccion + CT17DescuentoDependienteDeduccion + CT18InteresViviendaDeduccion;

            #endregion Total Deducciones

            C20subTotal3 = C15subTotal2 - PL16MedicinaPrepagada - C17DescuentoDependiente - PL18InteresVivienda;

            #region Renta exenta

            if (C20subTotal3 * valorRentaExenta > 240 * valorUvt)
            {
                C21RentaExenta = 240 * valorUvt;
            }
            else
            {
                C21RentaExenta = C20subTotal3 * valorRentaExenta;
            }

            C22LimiteRentaExenta = C12subTotal1 * valorLimiteRentaExenta;
            C23TotalRentaExenta = PL14Afc + PL16MedicinaPrepagada + C17DescuentoDependiente + PL18InteresVivienda + C21RentaExenta;

            #endregion Renta exenta

            #region Diferencial Renta 

            decimal CT24DiferenciaRenta = 0;

            decimal variableDiferencialRenta = PL24PensionVoluntaria + PL14Afc + C19TotalDeducciones + C21RentaExenta;
            if ((variableDiferencialRenta) > (C22LimiteRentaExenta))
            {
                CT24DiferenciaRenta = C22LimiteRentaExenta - (variableDiferencialRenta);
            }
            else
            {
                CT24DiferenciaRenta = 0;
            }

            #endregion Diferencial Renta 

            #region Base Gravable

            if (CT24DiferenciaRenta < 0)
            {
                CT24DiferenciaRenta = CT24DiferenciaRenta * (-1);
                C25BaseGravableRenta = C20subTotal3 - C21RentaExenta + CT24DiferenciaRenta;
            }
            else
            {
                C25BaseGravableRenta = C20subTotal3 - C21RentaExenta - CT24DiferenciaRenta;
            }

            C26BaseGravableRentaUvt = (C25BaseGravableRenta / valorUvt);
            C26BaseGravableRentaUvtFinal = (int)Math.Round(C26BaseGravableRentaUvt, 0, MidpointRounding.AwayFromZero);

            #endregion Base Gravable

            #region Setear valores a formato

            formato.BaseGravableRenta = C25BaseGravableRenta;
            formato.BaseGravableUvt = C26BaseGravableRentaUvtFinal;
            formato.BaseGravableUvtCalculada = C26BaseGravableRentaUvt;

            #endregion Setear valores a formato

            return formato;
        }


        #endregion Contrato Prestación de Servicio

        #region Proveedores con deducción

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(PlanPagoDto planPago,
                                                    ParametroLiquidacionTercero parametroLiquidacion,
                                                    List<ParametroGeneral> parametroGenerales,
                                                    List<CriterioCalculoReteFuente> listaCriterioReteFuente,
                                                    List<DeduccionDto> listaDeducciones)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();


            decimal PLTarifaIva = 0, baseGravableFinal = 0, baseGravableUvtCalculada = 0, valorUvt;
            decimal valor = 0;

            //Para Proveedores con deducción siempre es 30
            int baseGravableUvtFinal = 0, honorarioUvt = 0;

            decimal C30ValorIva = 0, C31Honorario = 0, C31HonorarioUvt = 0, CSubTotal1 = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLTarifaIva = parametroLiquidacion.TarifaIva;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);

            var parametrosCodigoRenta = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoRenta);
            var parametrosCodigoIva = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoIva);

            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales

            #region Calcular valores y obtener formato

            formato.ValorIva =
            formato.SubTotal3 = 0;
            formato.RentaExenta = 0;
            formato.LimiteRentaExenta = 0;
            formato.TotalRentaExenta = 0;
            formato.DiferencialRenta = 0;

            C31Honorario = planPago.ValorFacturado / (1 + PLTarifaIva);

            C30ValorIva = planPago.ValorFacturado - C31Honorario;

            C31HonorarioUvt = C31Honorario / valorUvt;
            honorarioUvt = (int)Math.Round(C31HonorarioUvt, 0, MidpointRounding.AwayFromZero);

            baseGravableFinal = C31Honorario;
            baseGravableUvtCalculada = C31Honorario / valorUvt;
            baseGravableUvtFinal = (int)Math.Round(baseGravableUvtCalculada, 0, MidpointRounding.AwayFromZero);

            formato.BaseGravableRenta = baseGravableFinal;
            formato.BaseGravableUvt = baseGravableUvtFinal;
            formato.ValorIva = C30ValorIva;
            formato.Honorario = C31Honorario;
            formato.HonorarioUvt = honorarioUvt;
            formato.ValorTotal = planPago.ValorFacturado;
            formato.BaseGravableUvtCalculada = baseGravableUvtCalculada;

            #endregion Calcular valores y obtener formato

            #region Obtener Lista de deducciones

            //temp
            CSubTotal1 = C31Honorario;

            if (listaDeducciones != null && listaDeducciones.Count > 0)
            {
                foreach (var deduccion in listaDeducciones)
                {
                    if (DeduccionEsParametroGeneral(parametrosCodigoRenta, deduccion.Codigo))
                    {
                        deduccion.Base = baseGravableFinal;
                        valor = deduccion.Tarifa * baseGravableFinal;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoIva, deduccion.Codigo))
                    {
                        deduccion.Base = C30ValorIva;
                        valor = deduccion.Tarifa * C30ValorIva;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if ((deduccion.TipoBaseDeduccionId != (int)TipoBaseDeducciones.OTRAS))
                    {
                        deduccion.Base = CSubTotal1;
                        valor = deduccion.Tarifa * CSubTotal1;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }

            #region Deducción Otras

            var deduccionOtras = listaDeducciones
                                    .Where(x => x.TipoBaseDeduccionId == (int)TipoBaseDeducciones.OTRAS)
                                    .FirstOrDefault();

            if (deduccionOtras != null)
            {
                var valorGmf = obtenerSumatoriaValorGMf(listaDeducciones);
                deduccionOtras.Base = valorGmf;
                valor = deduccionOtras.Tarifa * valorGmf;
                deduccionOtras.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
            }

            #endregion Deducción Otras

            formato.Deducciones = new List<DeduccionDto>();
            formato.Deducciones = listaDeducciones;

            #endregion Obtener lista de deducciones          

            formato.TotalRetenciones = obtenerSumatoriaRetenciones(listaDeducciones);
            formato.TotalAGirar = formato.ValorTotal - formato.TotalRetenciones;

            return formato;
        }


        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresConDeduccionVariable(PlanPagoDto planPago,
                                            FormatoSolicitudPago solicitudPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            List<CriterioCalculoReteFuente> listaCriterioReteFuente,
                                            List<DeduccionDto> listaDeducciones,
                                            decimal valorBaseGravable)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            decimal PLTarifaIva = 0, baseGravableFinal = 0, valorUvt;
            decimal valor = 0;

            //Para Proveedores con deducción siempre es 30
            int baseGravableUvtFinal = 0, honorarioUvt = 0;

            decimal C30ValorIva = 0, C31Honorario = 0, C31HonorarioUvt = 0, CSubTotal1 = 0, baseGravableUvtCalculada = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLTarifaIva = parametroLiquidacion.TarifaIva;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);

            var parametrosCodigoRenta = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoRenta);
            var parametrosCodigoIva = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoIva);

            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales

            #region Calcular valores y obtener formato

            formato.ValorIva =
            formato.SubTotal3 = 0;
            formato.RentaExenta = 0;
            formato.LimiteRentaExenta = 0;
            formato.TotalRentaExenta = 0;
            formato.DiferencialRenta = 0;

            if (parametroLiquidacion.TipoIva == (int)TipoDeIva.Fijo)
            {
                C30ValorIva = valorBaseGravable * PLTarifaIva;
            }
            else
            {
                C30ValorIva = solicitudPago.ValorIva;
            }

            C31Honorario = planPago.ValorFacturado - C30ValorIva;
            C31HonorarioUvt = C31Honorario / valorUvt;
            honorarioUvt = (int)Math.Round(C31HonorarioUvt, 0, MidpointRounding.AwayFromZero);

            baseGravableFinal = valorBaseGravable;
            baseGravableUvtCalculada = valorBaseGravable / valorUvt;
            baseGravableUvtFinal = (int)Math.Round(baseGravableUvtCalculada, 0, MidpointRounding.AwayFromZero);

            formato.BaseGravableRenta = baseGravableFinal;
            formato.BaseGravableUvt = baseGravableUvtFinal;
            formato.ValorIva = C30ValorIva;
            formato.Honorario = C31Honorario;
            formato.HonorarioUvt = honorarioUvt;
            formato.ValorTotal = planPago.ValorFacturado;

            #endregion Calcular valores y obtener formato

            #region Obtener Lista de deducciones            

            //temp
            CSubTotal1 = C31Honorario;

            if (listaDeducciones != null && listaDeducciones.Count > 0)
            {
                foreach (var deduccion in listaDeducciones)
                {
                    if (DeduccionEsParametroGeneral(parametrosCodigoRenta, deduccion.Codigo))
                    {
                        deduccion.Base = baseGravableFinal;
                        valor = deduccion.Tarifa * baseGravableFinal;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoIva, deduccion.Codigo))
                    {
                        deduccion.Base = C30ValorIva;
                        valor = deduccion.Tarifa * C30ValorIva;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if ((deduccion.TipoBaseDeduccionId != (int)TipoBaseDeducciones.OTRAS))
                    {
                        deduccion.Base = baseGravableFinal;
                        valor = deduccion.Tarifa * baseGravableFinal;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }

            #region Deducción Otras

            var deduccionOtras = listaDeducciones
                                    .Where(x => x.TipoBaseDeduccionId == (int)TipoBaseDeducciones.OTRAS)
                                    .FirstOrDefault();

            if (deduccionOtras != null)
            {
                var valorGmf = obtenerSumatoriaValorGMf(listaDeducciones);
                deduccionOtras.Base = valorGmf;
                valor = deduccionOtras.Tarifa * valorGmf;
                deduccionOtras.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
            }

            #endregion Deducción Otras

            formato.Deducciones = new List<DeduccionDto>();
            formato.Deducciones = listaDeducciones;

            #endregion Obtener lista de deducciones          

            formato.TotalRetenciones = obtenerSumatoriaRetenciones(listaDeducciones);
            formato.TotalAGirar = formato.ValorTotal - formato.TotalRetenciones;

            return formato;
        }

        #endregion Proveedores con deducción

        #region Proveeedores sin Deducción

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresSinDeduccion(PlanPagoDto planPago,
                                                          ParametroLiquidacionTercero parametroLiquidacion,
                                                          List<ParametroGeneral> parametroGenerales,
                                                          List<CriterioCalculoReteFuente> listaCriterioReteFuente)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            decimal PLTarifaIva = 0, baseGravable30 = 0, baseGravableFinal = 0, valorUvt;

            //Para Proveedores con deducción siempre es 30
            int baseGravableUvtFinal = 0, honorarioUvt = 0;

            decimal C30ValorIva = 0, C31Honorario = 0, C31HonorarioUvt = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLTarifaIva = parametroLiquidacion.TarifaIva;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);

            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales

            #region Calcular valores y obtener formato

            formato.ValorIva =
            formato.SubTotal3 = 0;
            formato.RentaExenta = 0;
            formato.LimiteRentaExenta = 0;
            formato.TotalRentaExenta = 0;
            formato.DiferencialRenta = 0;

            C31Honorario = (planPago.ValorFacturado) / (1 + PLTarifaIva);
            C30ValorIva = planPago.ValorFacturado - C31Honorario;

            C31HonorarioUvt = C31Honorario / valorUvt;
            honorarioUvt = (int)Math.Round(C31HonorarioUvt, 0, MidpointRounding.AwayFromZero);

            baseGravableFinal = C31Honorario;
            baseGravable30 = C31Honorario / valorUvt;
            baseGravableUvtFinal = (int)Math.Round(baseGravable30, 0, MidpointRounding.AwayFromZero);

            formato.BaseGravableRenta = baseGravableFinal;
            formato.BaseGravableUvt = baseGravableUvtFinal;
            formato.ValorIva = C30ValorIva;
            formato.Honorario = C31Honorario;
            formato.HonorarioUvt = honorarioUvt;
            formato.ValorTotal = planPago.ValorFacturado;

            #endregion Calcular valores y obtener formato

            formato.TotalRetenciones = 0;
            formato.TotalAGirar = formato.ValorTotal;

            return formato;
        }


        #endregion Proveedores sin deducción

        #endregion Liquidación Normal

        #region Liquidación Masiva

        public async Task<bool> RegistrarListaDetalleLiquidacion(int usuarioId, int pciId,
                                                                string listaPlanPagoId,
                                                                List<int> listIds,
                                                                bool esSeleccionarTodo, int? terceroId)
        {
            #region Variables 

            List<LiquidacionDeduccion> listaLiquidacionDeduccionARegistrar = new List<LiquidacionDeduccion>();

            #endregion Variables 

            #region Obtener Parametros generales

            var listaParametros = await _repoLista.ObtenerParametrosGenerales();
            listaParametrosGenerales = listaParametros.ToList();

            var listaCriterio = await _repoLista.ObtenerListaCriterioCalculoReteFuente();
            listaCriterioCalculoReteFuente = listaCriterio.ToList();

            #endregion Obtener Parametros generales

            if (esSeleccionarTodo)
            {
                #region esSeleccionarTodo

                UserParams userParams = new UserParams();
                userParams.PageSize = 500;
                userParams.PageNumber = 1;
                userParams.PciId = pciId;

                var pagedList = await _planPagoRepository.ObtenerListaPlanPago(terceroId, listIds, userParams);

                for (int i = 1; i <= pagedList.TotalPages; i++)
                {
                    userParams.PageNumber = i;

                    //Obtener lista de planes de pagos a procesar: 500 items x Lote
                    var paginacion = await _planPagoRepository.ObtenerListaPlanPago(terceroId, listIds, userParams);
                    var lista = _mapper.Map<IEnumerable<PlanPago>>(paginacion);

                    //Filtrar lista de planes de pago
                    //listaPlanPago = FiltrarListaPlanesPago(lista);

                    await ProcesarLiquidacionMasiva(usuarioId, pciId, lista.ToList());
                }

                #endregion esSeleccionarTodo
            }
            else
            {
                #region Procesar por lista de ids

                if (!string.IsNullOrEmpty(listaPlanPagoId))
                {
                    List<int> listaPlanPagoIds = listaPlanPagoId.Split(',').Select(int.Parse).ToList();
                    var lista = await _planPagoRepository.ObtenerListaPlanPagoXIds(listaPlanPagoIds);

                    //Filtrar lista de planes de pago
                    //listaPlanPago = FiltrarListaPlanesPago(lista);

                    await ProcesarLiquidacionMasiva(usuarioId, pciId, lista);
                }

                #endregion Procesar por lista de ids
            }

            return true;
        }


        public List<DetalleLiquidacion> ObtenerListaDetalleLiquidacionARegistrar(int usuarioId,
                                                                                    List<PlanPago> listaPlanPago,
                                                                                    List<FormatoSolicitudPago> listaSolicitudPago,
                                                                                    List<DetallePlanPagoDto> listaDetallePlanPago,
                                                                                    List<ParametroLiquidacionTercero> listaParametroLiquidacionTercero,
                                                                                    List<TerceroDeduccion> listaDeduccionesXTercero,
                                                                                    List<DetalleLiquidacion> listaDetalleLiquidacionMesAnteriorViatico,
                                                                                    List<DetalleLiquidacion> listaDetalleLiquidacionMesAnterior)
        {
            DetalleLiquidacion detalleLiquidacion = null;
            List<DetalleLiquidacion> listaDetalleLiquidacion = new List<DetalleLiquidacion>();

            FormatoCausacionyLiquidacionPagos formato = null;
            List<DeduccionDto> listaDeduccionesDto = null;
            ParametroLiquidacionTercero parametroLiquidacionTercero = null;
            DetallePlanPagoDto detallePlanPago = null;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();
            DetalleLiquidacion detalleLiquidacionMesAnterior = null;
            List<DetalleLiquidacion> listaLiquidacionMesAnteriorViaticoXTercero = null;
            FormatoSolicitudPago solicitudPago = null;
            List<TerceroDeduccion> listaDeduccionesXTerceroyActividadEconomica = null;
            decimal valorBaseGravable = 0;
            PlanPagoDto planPagoDto = null;

            try
            {
                if (listaPlanPago != null && listaPlanPago.Count > 0)
                {
                    foreach (var planPago in listaPlanPago)
                    {
                        try
                        {
                            detalleLiquidacion = new DetalleLiquidacion();

                            planPagoDto = new PlanPagoDto();
                            planPagoDto.ValorFacturado = planPago.ValorFacturado.Value;

                            parametroLiquidacionTercero = listaParametroLiquidacionTercero.Where(x => x.TerceroId == planPago.TerceroId).FirstOrDefault();
                            detallePlanPago = listaDetallePlanPago.Where(x => x.PlanPagoId == planPago.PlanPagoId).FirstOrDefault();
                            solicitudPago = listaSolicitudPago.Where(x => x.PlanPagoId == planPago.PlanPagoId).FirstOrDefault();

                            if (detallePlanPago != null)
                            {
                                //Filtrar lista de deducciones por actividad economica en la solicitud de pago
                                listaDeduccionesXTerceroyActividadEconomica = FiltrarListaDeduccionesXTerceroyActividadEconomica(listaDeduccionesXTercero, solicitudPago);

                                var listaDeduccion = listaDeduccionesXTerceroyActividadEconomica
                                                    .Where(x => x.TerceroId == planPago.TerceroId)
                                                    .Select(x => x.Deduccion)
                                                    .ToList();

                                var lista = _mapper.Map<ICollection<DeduccionDto>>(listaDeduccion);
                                listaDeduccionesDto = lista.ToList();

                                if (planPago.Tercero.ModalidadContrato == (int)ModalidadContrato.ContratoPrestacionServicio)
                                {
                                    if (!parametroLiquidacionTercero.Subcontrata)
                                    {
                                        if (!planPago.Viaticos)
                                        {
                                            if (listaDetalleLiquidacionMesAnterior != null && listaDetalleLiquidacionMesAnterior.Count > 0)
                                            {
                                                detalleLiquidacionMesAnterior = listaDetalleLiquidacionMesAnterior
                                                                                .Where(x => x.TerceroId == planPago.TerceroId)
                                                                                .FirstOrDefault();
                                            }
                                            if (listaDetalleLiquidacionMesAnteriorViatico != null && listaDetalleLiquidacionMesAnteriorViatico.Count > 0)
                                            {
                                                listaLiquidacionMesAnteriorViaticoXTercero = listaDetalleLiquidacionMesAnteriorViatico
                                                                                            .Where(x => x.TerceroId == planPago.TerceroId)
                                                                                            .ToList();
                                            }

                                            formato = ObtenerFormatoCausacion_ContratoPrestacionServicioMasivo(planPago, parametroLiquidacionTercero,
                                                                                        listaParametrosGenerales, listaCriterioCalculoReteFuente,
                                                                                        listaDeduccionesDto.ToList(),
                                                                                        listaLiquidacionMesAnteriorViaticoXTercero,
                                                                                        detalleLiquidacionMesAnterior);
                                        }
                                        else
                                        {
                                            //Contratistas con viaticos liquida de otra manera
                                            formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto, parametroLiquidacionTercero,
                                                                                                        listaParametrosGenerales, listaCriterioCalculoReteFuente);
                                        }
                                    }
                                    else
                                    {
                                        formato = ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(planPagoDto, parametroLiquidacionTercero,
                                                                                                        listaParametrosGenerales,
                                                                                                        listaCriterioCalculoReteFuente,
                                                                                                        listaDeduccionesDto.ToList());
                                    }
                                }
                                else if (planPago.Tercero.ModalidadContrato == (int)ModalidadContrato.ProveedorConDescuento)
                                {

                                    if (planPago.Tercero.TipoPago == (int)TipoPago.Fijo)
                                    {
                                        formato = ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(planPagoDto, parametroLiquidacionTercero,
                                                                                                        listaParametrosGenerales,
                                                                                                        listaCriterioCalculoReteFuente,
                                                                                                        listaDeduccionesDto.ToList());
                                    }
                                    else
                                    {
                                        valorBaseGravable = solicitudPago.ValorBaseGravableRenta;

                                        formato = ObtenerFormatoCausacion_ProveedoresConDeduccionVariable(planPagoDto, solicitudPago, parametroLiquidacionTercero,
                                                                                    listaParametrosGenerales, listaCriterioCalculoReteFuente,
                                                                                    listaDeduccionesDto.ToList(), valorBaseGravable);
                                    }

                                }
                                else if (planPago.Tercero.ModalidadContrato == (int)ModalidadContrato.ProveedorSinDescuento)
                                {
                                    formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto, parametroLiquidacionTercero,
                                                                                              listaParametrosGenerales,
                                                                                              listaCriterioCalculoReteFuente);
                                }

                                #region Mapear datos

                                MapearFormatoLiquidacionPlanPago(detallePlanPago, detalleLiquidacion);
                                formato.TextoComprobanteContable = detallePlanPago.TextoComprobanteContable;
                                MapearFormatoLiquidacion(formato, detalleLiquidacion);

                                detalleLiquidacion.MesSaludActual = fechaActual.Month;
                                detalleLiquidacion.MesPagoActual = fechaActual.Month;
                                detalleLiquidacion.ModalidadContrato = planPago.Tercero.ModalidadContrato;
                                detalleLiquidacion.UsuarioIdRegistro = usuarioId;
                                detalleLiquidacion.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                                MapearLiquidacionDeducciones(formato, detalleLiquidacion);

                                #endregion Mapear datos

                                listaDetalleLiquidacion.Add(detalleLiquidacion);
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                return listaDetalleLiquidacion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Contrato Prestación de Servicio

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ContratoPrestacionServicioMasivo(PlanPago planPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            List<CriterioCalculoReteFuente> listaCriterioReteFuente,
                                            List<DeduccionDto> listaDeducciones,
                                            List<DetalleLiquidacion> listaDetalleLiquidacionMesAnteriorViatico,
                                            DetalleLiquidacion detalleLiquidacionMesAnterior)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual30 = new FormatoCausacionyLiquidacionPagos();
            CriterioCalculoReteFuente criterioReteFuente = null;

            decimal C30ValorIva = 0, PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0, baseGravableFinal = 0,
            valorUvt, baseGravableUvtCalculada = 0, factorCalculo = 0;
            int C32NumeroDiaLaborados = 0, baseGravableUvtFinal = 0;

            decimal valor = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PL17HonorarioSinIva = ObtenerHonorarioSinIva(parametroLiquidacion, planPago.ValorFacturado.Value);
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;

            var tarifaIva = parametroLiquidacion.TarifaIva;
            var GmfAfc = parametroLiquidacion.GmfAfc;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);

            var parametrosCodigoRenta = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoRenta);
            var parametrosCodigoIva = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoIva);
            var parametrosCodigoPensionVoluntaria = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoPensionVoluntaria);
            var parametrosCodigoAFC = obtenerParametrosGeneralesXTipo(parametroGenerales, codigoAFC);

            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales           

            #region Numero de dias laborados

            if (PL17HonorarioSinIva > 0)
            {
                valor = (((planPago.ValorFacturado.Value / (1 + tarifaIva)) * 30) / PL17HonorarioSinIvaParametro);
                C32NumeroDiaLaborados = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                factorCalculo = ((decimal)C32NumeroDiaLaborados / (decimal)30);
            }

            #endregion Numero de dias laborados

            #region Calcular valores y obtener formato

            formatoIgual = CalcularValoresFormatoContratoPrestacionServicioMasivo(planPago, parametroLiquidacion,
                                                                                parametroGenerales, numeroDiasLaborados: 30,
                                                                                listaDetalleLiquidacionMesAnteriorViatico,
                                                                                detalleLiquidacionMesAnterior,
                                                                                factorCalculo);

            formatoIgual30 = CalcularValoresBaseGravableParaContratoPrestacionServicioMasivo(planPago, parametroLiquidacion, parametroGenerales, listaDetalleLiquidacionMesAnteriorViatico);

            formato = formatoIgual;
            baseGravableUvtCalculada = formatoIgual30.BaseGravableUvtCalculada;
            baseGravableFinal = formatoIgual30.BaseGravableRenta;
            baseGravableUvtFinal = formatoIgual30.BaseGravableUvt;

            formato.BaseGravableUvtCalculada = baseGravableUvtCalculada;
            formato.BaseGravableRenta = baseGravableFinal;
            formato.BaseGravableUvt = baseGravableUvtFinal;

            #endregion Calcular valores y obtener formato

            #region Obtener Lista de deducciones

            C30ValorIva = formato.ValorIva;

            decimal valorMinimoRango = 0;
            decimal factorIncremento = 0;
            decimal tarifaCalculo = 0;

            criterioReteFuente = ObtenerCriterioCalculoRendimiento(listaCriterioReteFuente, baseGravableUvtCalculada);

            valorMinimoRango = criterioReteFuente.Desde;
            factorIncremento = criterioReteFuente.Factor;
            tarifaCalculo = criterioReteFuente.Tarifa;

            if (listaDeducciones != null && listaDeducciones.Count > 0)
            {
                foreach (var deduccion in listaDeducciones)
                {
                    if (DeduccionEsParametroGeneral(parametrosCodigoRenta, deduccion.Codigo))
                    {
                        deduccion.Base = baseGravableFinal;
                        var valorRentaCalculado = ((((tarifaCalculo / 100) * (baseGravableUvtCalculada - valorMinimoRango))
                                                        + factorIncremento) * valorUvt) * factorCalculo;
                        deduccion.Valor = this._generalInterface.ObtenerValorRedondeadoCPS(valorRentaCalculado);

                        if (deduccion.Base > 0)
                        {
                            deduccion.Tarifa = deduccion.Valor / deduccion.Base;
                        }
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoIva, deduccion.Codigo))
                    {
                        deduccion.Base = C30ValorIva;
                        valor = deduccion.Tarifa * C30ValorIva;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);

                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoAFC, deduccion.Codigo))
                    {
                        deduccion.Base = formato.Afc;
                        valor = deduccion.Tarifa * deduccion.Base;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (DeduccionEsParametroGeneral(parametrosCodigoPensionVoluntaria, deduccion.Codigo))
                    {
                        deduccion.Base = formato.PensionVoluntaria;
                        valor = deduccion.Tarifa * deduccion.Base;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                    else if ((deduccion.TipoBaseDeduccionId != (int)TipoBaseDeducciones.OTRAS))
                    {
                        deduccion.Base = formato.SubTotal1;
                        valor = deduccion.Tarifa * formato.SubTotal1;
                        deduccion.Valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }

            #region Deducción Otras

            var deduccionOtras = listaDeducciones
                                    .Where(x => x.TipoBaseDeduccionId == (int)TipoBaseDeducciones.OTRAS)
                                    .FirstOrDefault();

            if (deduccionOtras != null)
            {
                var valorGmf = obtenerSumatoriaValorGMf(listaDeducciones, parametrosCodigoAFC, GmfAfc);
                deduccionOtras.Base = valorGmf;
                valor = deduccionOtras.Tarifa * valorGmf;
                valor = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
                deduccionOtras.Valor = valor;

            }

            #endregion Deducción Otras

            formato.Deducciones = new List<DeduccionDto>();
            formato.Deducciones = listaDeducciones;

            #endregion Obtener lista de deducciones          

            formato.TotalRetenciones = obtenerSumatoriaRetenciones(listaDeducciones);
            formato.TotalAGirar = formato.ValorTotal - formato.TotalRetenciones;

            return formato;
        }



        private FormatoCausacionyLiquidacionPagos CalcularValoresFormatoContratoPrestacionServicioMasivo(
                                            PlanPago planPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            int numeroDiasLaborados,
                                            List<DetalleLiquidacion> listaDetalleLiquidacionMesAnteriorViatico,
                                            DetalleLiquidacion detalleLiquidacionMesAnterior,
                                            decimal factorCalculo)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            DateTime? fechaInicio = null, fechaFinal = null;

            decimal valorUvt = 0, valorSMLV = 0;
            decimal valorDescuentoDependiente = 0.10m;
            decimal valorRentaExenta = 0.25m;
            decimal valorLimiteRentaExenta = 0.4m;
            decimal valorDiferencialRenta = 0.3m;

            decimal PLtarifaIva = 0, PLbaseAporteSalud = 0, PLaporteSalud = 0,
            PLaportePension = 0, PLriesgoLaboral = 0, PLfondoSolidaridad = 0,
            //PL13PensionVoluntaria = 0, 
            PL14Afc = 0, PL16MedicinaPrepagada = 0,
            PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0, PL17DescuentoDependiente = 0,
            PL18InteresVivienda = 0, PL24PensionVoluntaria = 0, viaticosPagados = 0;

            decimal C1honorario = 0, C2honorarioUvt = 0, C3valorIva = 0,
            C8aporteASalud = 0, C9aporteAPension = 0, C10aporteRiesgoLaboral = 0,
            C7baseAporteSalud = 0, C11fondoSolidaridad, C12subTotal1 = 0,
            C15subTotal2 = 0, C20subTotal3 = 0, C17DescuentoDependiente = 0,
            C19TotalDeducciones = 0, C21RentaExenta = 0, C22LimiteRentaExenta = 0,
            C23TotalRentaExenta = 0, C24DiferencialRenta = 0,
            C25BaseGravableRenta = 0, C26BaseGravableRentaUvt = 0;

            int C26BaseGravableRentaUvtFinal = 0, C2honorarioUvtFinal = 0;

            decimal cuatroSMLV = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLtarifaIva = parametroLiquidacion.TarifaIva;
            PLbaseAporteSalud = parametroLiquidacion.BaseAporteSalud;
            PLaporteSalud = parametroLiquidacion.AporteSalud;
            PLaportePension = parametroLiquidacion.AportePension;
            PLriesgoLaboral = parametroLiquidacion.RiesgoLaboral;
            PLfondoSolidaridad = parametroLiquidacion.FondoSolidaridad;
            PL14Afc = parametroLiquidacion.Afc;
            PL16MedicinaPrepagada = parametroLiquidacion.MedicinaPrepagada;
            PL17HonorarioSinIva = ObtenerHonorarioSinIva(parametroLiquidacion, planPago.ValorFacturado.Value);
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;
            PL17DescuentoDependiente = parametroLiquidacion.Dependiente;
            PL18InteresVivienda = parametroLiquidacion.InteresVivienda;
            PL24PensionVoluntaria = parametroLiquidacion.PensionVoluntaria;

            if (factorCalculo == 1)
            {
                PL17HonorarioSinIva = PL17HonorarioSinIvaParametro;
            }

            if (parametroLiquidacion.FechaInicioDescuentoInteresVivienda.HasValue)
                fechaInicio = parametroLiquidacion.FechaInicioDescuentoInteresVivienda.Value;

            if (parametroLiquidacion.FechaFinalDescuentoInteresVivienda.HasValue)
                fechaFinal = parametroLiquidacion.FechaFinalDescuentoInteresVivienda.Value;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            #region Viaticos y Aporte Salud Anterior

            if (listaDetalleLiquidacionMesAnteriorViatico != null && listaDetalleLiquidacionMesAnteriorViatico.Count > 0)
            {
                viaticosPagados = ObtenerValorViaticosAnteriores(listaDetalleLiquidacionMesAnteriorViatico.ToList());
            }

            if (detalleLiquidacionMesAnterior != null)
            {
                formato.NumeroMesSaludAnterior = detalleLiquidacionMesAnterior.MesSaludAnterior;
                formato.MesSaludAnterior = (detalleLiquidacionMesAnterior.MesSaludAnterior > 0 && detalleLiquidacionMesAnterior.MesSaludAnterior < 13) ?
                                            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(detalleLiquidacionMesAnterior.MesSaludAnterior).ToUpper() :
                                            string.Empty;
            }

            #endregion Viáticos y Aporte Salud Anterior

            #region Honorario

            C1honorario = PL17HonorarioSinIva + viaticosPagados;

            #endregion Honorario 

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C3valorIva = C1honorario * PLtarifaIva;
            C7baseAporteSalud = (C1honorario - viaticosPagados) * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                //Se cambia de forma temporal, debe ser 4
                cuatroSMLV = 2 * valorSMLV;
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

            C12subTotal1 = C1honorario - C8aporteASalud - C9aporteAPension - C10aporteRiesgoLaboral - C11fondoSolidaridad;
            C15subTotal2 = C12subTotal1 - PL24PensionVoluntaria - PL14Afc;

            #region Descuento Dependiente

            if (PL17HonorarioSinIvaParametro * valorDescuentoDependiente > valorUvt * 32)
            {
                if (PL17DescuentoDependiente > 0)
                {
                    C17DescuentoDependiente = valorUvt * 32;
                }
            }
            else
            {
                C17DescuentoDependiente = PL17HonorarioSinIvaParametro * PL17DescuentoDependiente;
            }

            #endregion Descuento Dependiente

            #region Interes de vivienda

            if (fechaInicio != null && fechaFinal != null)
            {
                if (!FechaActualEntreFechasVigencia(fechaInicio.Value, fechaFinal.Value))
                {
                    PL18InteresVivienda = 0;
                }
            }

            #endregion Interes de vivienda

            #region Total Deducciones

            decimal CT16MedicinaPrepagadaDeduccion = 0, CT17DescuentoDependienteDeduccion = 0,
            CT18InteresViviendaDeduccion = 0;

            //Deduccion Medicina Prepagada
            if (PL16MedicinaPrepagada > valorUvt * 16)
            {
                CT16MedicinaPrepagadaDeduccion = valorUvt * 16;
            }
            else
            {
                CT16MedicinaPrepagadaDeduccion = PL16MedicinaPrepagada;
            }

            //Deducción Descuento Dependiente
            if (PL17DescuentoDependiente > valorUvt * 32)
            {
                CT17DescuentoDependienteDeduccion = valorUvt * 32;
            }
            else
            {
                CT17DescuentoDependienteDeduccion = C17DescuentoDependiente;
            }

            //Deducción Interes Vivienda
            if (PL18InteresVivienda > valorUvt * 100)
            {
                CT18InteresViviendaDeduccion = valorUvt * 100;
            }
            else
            {
                CT18InteresViviendaDeduccion = PL18InteresVivienda;
            }

            C19TotalDeducciones = CT16MedicinaPrepagadaDeduccion + CT17DescuentoDependienteDeduccion + CT18InteresViviendaDeduccion;

            #endregion Total Deducciones

            C20subTotal3 = C15subTotal2 - PL16MedicinaPrepagada - C17DescuentoDependiente - PL18InteresVivienda;

            #region Renta exenta

            if (C20subTotal3 * valorRentaExenta > 240 * valorUvt)
            {
                C21RentaExenta = 240 * valorUvt;
            }
            else
            {
                C21RentaExenta = C20subTotal3 * valorRentaExenta;
            }

            C22LimiteRentaExenta = C12subTotal1 * valorLimiteRentaExenta;
            C23TotalRentaExenta = PL14Afc + PL16MedicinaPrepagada + C17DescuentoDependiente + PL18InteresVivienda + C21RentaExenta;

            #endregion Renta exenta

            #region Diferencial Renta 

            decimal CT24DiferenciaRenta = 0;

            if ((PL24PensionVoluntaria + PL14Afc) > (C1honorario * valorDiferencialRenta))
            {
                CT24DiferenciaRenta = C1honorario * valorDiferencialRenta;
            }
            else
            {
                CT24DiferenciaRenta = PL24PensionVoluntaria + PL14Afc;
            }

            C24DiferencialRenta = CT24DiferenciaRenta + C19TotalDeducciones + C21RentaExenta;

            if (C24DiferencialRenta > C22LimiteRentaExenta)
            {
                C24DiferencialRenta = C22LimiteRentaExenta - C24DiferencialRenta;
            }
            else
            {
                C24DiferencialRenta = 0;
            }

            #endregion Diferencial Renta 

            #region Base Gravable

            C25BaseGravableRenta = (C20subTotal3 - C21RentaExenta - C24DiferencialRenta);
            C26BaseGravableRentaUvt = (C25BaseGravableRenta / valorUvt) * factorCalculo;
            C26BaseGravableRentaUvtFinal = (int)Math.Round(C26BaseGravableRentaUvt, 0, MidpointRounding.AwayFromZero);

            #endregion Base Gravable

            #region Setear valores a formato

            formato.Honorario = C1honorario;
            formato.HonorarioUvt = C2honorarioUvtFinal;
            formato.ValorIva = C3valorIva;
            formato.ValorTotal = planPago.ValorFacturado.Value;
            formato.BaseSalud = C7baseAporteSalud;
            formato.AporteSalud = C8aporteASalud;
            formato.AportePension = C9aporteAPension;
            formato.RiesgoLaboral = C10aporteRiesgoLaboral;
            formato.FondoSolidaridad = C11fondoSolidaridad;
            formato.SubTotal1 = C12subTotal1;
            formato.Afc = PL14Afc;
            formato.PensionVoluntaria = PL24PensionVoluntaria;
            formato.SubTotal2 = C15subTotal2;
            formato.MedicinaPrepagada = PL16MedicinaPrepagada;
            formato.Dependientes = C17DescuentoDependiente;
            formato.SubTotal3 = C20subTotal3;
            formato.RentaExenta = C21RentaExenta;
            formato.LimiteRentaExenta = C22LimiteRentaExenta;
            formato.TotalRentaExenta = C23TotalRentaExenta;
            formato.InteresVivienda = CT18InteresViviendaDeduccion;
            formato.TotalDeducciones = C19TotalDeducciones;
            formato.DiferencialRenta = C24DiferencialRenta;
            formato.BaseGravableRenta = C25BaseGravableRenta;
            formato.BaseGravableUvt = C26BaseGravableRentaUvtFinal;
            formato.BaseGravableUvtCalculada = C26BaseGravableRentaUvt;
            formato.ViaticosPagados = viaticosPagados;

            #endregion Setear valores a formato

            return formato;
        }

        private FormatoCausacionyLiquidacionPagos CalcularValoresBaseGravableParaContratoPrestacionServicioMasivo(
                                                          PlanPago planPago,
                                                          ParametroLiquidacionTercero parametroLiquidacion,
                                                          List<ParametroGeneral> parametroGenerales,
                                                          List<DetalleLiquidacion> listaDetalleLiquidacionMesAnteriorViatico)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();

            DateTime? fechaInicio = null, fechaFinal = null;

            decimal valorUvt = 0, valorSMLV = 0;
            decimal valorDescuentoDependiente = 0.10m;
            decimal valorRentaExenta = 0.25m;
            decimal valorLimiteRentaExenta = 0.4m;

            decimal PLtarifaIva = 0, PLbaseAporteSalud = 0, PLaporteSalud = 0,
            PLaportePension = 0, PLriesgoLaboral = 0, PLfondoSolidaridad = 0,
            PL14Afc = 0, PL16MedicinaPrepagada = 0,
            PL17HonorarioSinIva = 0, PL17HonorarioSinIvaParametro = 0, PL17DescuentoDependiente = 0,
            PL18InteresVivienda = 0, PL24PensionVoluntaria = 0, viaticosPagados = 0;

            decimal C1honorario = 0, C2honorarioUvt = 0, C3valorIva = 0,
            C8aporteASalud = 0, C9aporteAPension = 0, C10aporteRiesgoLaboral = 0,
            C7baseAporteSalud = 0, C11fondoSolidaridad, C12subTotal1 = 0,
            C15subTotal2 = 0, C20subTotal3 = 0, C17DescuentoDependiente = 0,
            C19TotalDeducciones = 0, C21RentaExenta = 0, C22LimiteRentaExenta = 0,
            C23TotalRentaExenta = 0, C25BaseGravableRenta = 0, C26BaseGravableRentaUvt = 0;

            int C26BaseGravableRentaUvtFinal = 0, C2honorarioUvtFinal = 0;

            decimal cuatroSMLV = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PLtarifaIva = parametroLiquidacion.TarifaIva;
            PLbaseAporteSalud = parametroLiquidacion.BaseAporteSalud;
            PLaporteSalud = parametroLiquidacion.AporteSalud;
            PLaportePension = parametroLiquidacion.AportePension;
            PLriesgoLaboral = parametroLiquidacion.RiesgoLaboral;
            PLfondoSolidaridad = parametroLiquidacion.FondoSolidaridad;
            PL14Afc = parametroLiquidacion.Afc;
            PL16MedicinaPrepagada = parametroLiquidacion.MedicinaPrepagada;
            PL17HonorarioSinIvaParametro = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;
            PL17HonorarioSinIva = PL17HonorarioSinIvaParametro;

            PL17DescuentoDependiente = parametroLiquidacion.Dependiente;
            PL18InteresVivienda = parametroLiquidacion.InteresVivienda;
            PL24PensionVoluntaria = parametroLiquidacion.PensionVoluntaria;

            if (parametroLiquidacion.FechaInicioDescuentoInteresVivienda.HasValue)
                fechaInicio = parametroLiquidacion.FechaInicioDescuentoInteresVivienda.Value;

            if (parametroLiquidacion.FechaFinalDescuentoInteresVivienda.HasValue)
                fechaFinal = parametroLiquidacion.FechaFinalDescuentoInteresVivienda.Value;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = ObtenerValorDeParametroGeneral(parametroGenerales, valorUVT);
            var parametroSMLV = ObtenerValorDeParametroGeneral(parametroGenerales, salarioMinimo);
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            #region Viaticos y Aporte Salud Anterior

            if (listaDetalleLiquidacionMesAnteriorViatico != null && listaDetalleLiquidacionMesAnteriorViatico.Count > 0)
            {
                viaticosPagados = ObtenerValorViaticosAnteriores(listaDetalleLiquidacionMesAnteriorViatico.ToList());
            }

            #endregion Viáticos y Aporte Salud Anterior

            #region Honorario 

            C1honorario = PL17HonorarioSinIva + viaticosPagados;

            #endregion Honorario 

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C3valorIva = C1honorario * PLtarifaIva;
            C7baseAporteSalud = (C1honorario - viaticosPagados) * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = this._generalInterface.ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                //Se cambia de forma temporal, debe ser 4
                cuatroSMLV = 2 * valorSMLV;
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

            C12subTotal1 = C1honorario - C8aporteASalud - C9aporteAPension - C10aporteRiesgoLaboral - C11fondoSolidaridad;
            C15subTotal2 = C12subTotal1 - PL24PensionVoluntaria - PL14Afc;

            #region Descuento Dependiente

            if (PL17HonorarioSinIvaParametro * valorDescuentoDependiente > valorUvt * 32)
            {
                if (PL17DescuentoDependiente > 0)
                {
                    C17DescuentoDependiente = valorUvt * 32;
                }
            }
            else
            {
                C17DescuentoDependiente = PL17HonorarioSinIvaParametro * PL17DescuentoDependiente;
            }

            #endregion Descuento Dependiente

            #region Interes de vivienda

            if (fechaInicio != null && fechaFinal != null)
            {
                if (!FechaActualEntreFechasVigencia(fechaInicio.Value, fechaFinal.Value))
                {
                    PL18InteresVivienda = 0;
                }
            }

            #endregion Interes de vivienda

            #region Total Deducciones

            decimal CT16MedicinaPrepagadaDeduccion = 0, CT17DescuentoDependienteDeduccion = 0,
            CT18InteresViviendaDeduccion = 0;

            //Deduccion Medicina Prepagada
            if (PL16MedicinaPrepagada > valorUvt * 16)
            {
                CT16MedicinaPrepagadaDeduccion = valorUvt * 16;
            }
            else
            {
                CT16MedicinaPrepagadaDeduccion = PL16MedicinaPrepagada;
            }

            //Deducción Descuento Dependiente
            if (PL17DescuentoDependiente > valorUvt * 32)
            {
                CT17DescuentoDependienteDeduccion = valorUvt * 32;
            }
            else
            {
                CT17DescuentoDependienteDeduccion = C17DescuentoDependiente;
            }

            //Deducción Interes Vivienda
            if (PL18InteresVivienda > valorUvt * 100)
            {
                CT18InteresViviendaDeduccion = valorUvt * 100;
            }
            else
            {
                CT18InteresViviendaDeduccion = PL18InteresVivienda;
            }

            C19TotalDeducciones = CT16MedicinaPrepagadaDeduccion + CT17DescuentoDependienteDeduccion + CT18InteresViviendaDeduccion;

            #endregion Total Deducciones

            C20subTotal3 = C15subTotal2 - PL16MedicinaPrepagada - C17DescuentoDependiente - PL18InteresVivienda;

            #region Renta exenta

            if (C20subTotal3 * valorRentaExenta > 240 * valorUvt)
            {
                C21RentaExenta = 240 * valorUvt;
            }
            else
            {
                C21RentaExenta = C20subTotal3 * valorRentaExenta;
            }

            C22LimiteRentaExenta = C12subTotal1 * valorLimiteRentaExenta;
            C23TotalRentaExenta = PL14Afc + PL16MedicinaPrepagada + C17DescuentoDependiente + PL18InteresVivienda + C21RentaExenta;

            #endregion Renta exenta

            #region Diferencial Renta 

            decimal CT24DiferenciaRenta = 0;

            decimal variableDiferencialRenta = PL24PensionVoluntaria + PL14Afc + C19TotalDeducciones + C21RentaExenta;
            if ((variableDiferencialRenta) > (C22LimiteRentaExenta))
            {
                CT24DiferenciaRenta = C22LimiteRentaExenta - (variableDiferencialRenta);
            }
            else
            {
                CT24DiferenciaRenta = 0;
            }

            #endregion Diferencial Renta 

            #region Base Gravable

            if (CT24DiferenciaRenta < 0)
            {
                CT24DiferenciaRenta = CT24DiferenciaRenta * (-1);
                C25BaseGravableRenta = C20subTotal3 - C21RentaExenta + CT24DiferenciaRenta;
            }
            else
            {
                C25BaseGravableRenta = C20subTotal3 - C21RentaExenta - CT24DiferenciaRenta;
            }

            C26BaseGravableRentaUvt = (C25BaseGravableRenta / valorUvt);
            C26BaseGravableRentaUvtFinal = (int)Math.Round(C26BaseGravableRentaUvt, 0, MidpointRounding.AwayFromZero);

            #endregion Base Gravable

            #region Setear valores a formato

            formato.BaseGravableRenta = C25BaseGravableRenta;
            formato.BaseGravableUvt = C26BaseGravableRentaUvtFinal;
            formato.BaseGravableUvtCalculada = C26BaseGravableRentaUvt;

            #endregion Setear valores a formato

            return formato;
        }

        #endregion Contrato Prestación de Servicio

        #endregion Liquidación Masiva

        #region Funciones Generales

        #region Liquidación Masiva

        private async Task ProcesarLiquidacionMasiva(int usuarioId, int pciId, List<PlanPago> listaPlanPago)
        {
            List<FormatoSolicitudPago> listaSolicitudPago = null;
            List<ParametroLiquidacionTercero> listaParametroLiquidacionTercero = null;
            List<TerceroDeduccion> listaDeduccionesXTercero = null;
            List<DetalleLiquidacion> listaDetalleLiquidacionMesAnteriorViatico = null;
            List<DetalleLiquidacion> listaDetalleLiquidacionMesAnterior = null;
            List<DetallePlanPagoDto> listaDetallePlanPago = null;
            List<DetallePlanPagoDto> listaCantidadMaximaXPlanPago = null;
            List<DetalleLiquidacion> listaDetalleLiquidacionARegistrar = null;
            List<LiquidacionDeduccion> listaLiquidacionDeduccionARegistrar = new List<LiquidacionDeduccion>();
            List<long> compromisos = new List<long>();
            int planPagoEstado_ConLiquidacionDeducciones = (int)EstadoPlanPago.ConLiquidacionDeducciones;

            #region Obtener listas secundarias

            var planPagoIds = listaPlanPago.Select(x => x.PlanPagoId).ToHashSet().ToList();

            //Obtener lista de detalle de plan de pago            
            listaDetallePlanPago = await ObtenerListaDetallePlanPagoXIds(planPagoIds);

            compromisos = listaDetallePlanPago.Select(x => x.Crp).ToHashSet().ToList();
            listaCantidadMaximaXPlanPago = await ObtenerListaCantidadMaximaPlanPago(compromisos, pciId);
            ActualizarListaDetallePlanPago(listaDetallePlanPago, listaCantidadMaximaXPlanPago);

            var terceroIds = listaPlanPago.Select(x => x.TerceroId).ToHashSet().ToList();
            //Obtener lista de parametros de liquidacion de terceros
            listaParametroLiquidacionTercero = await ObtenerListaParametroLiquidacionTerceroXIds(terceroIds, pciId);

            //Obtener lista de solicitudes de pago
            listaSolicitudPago = await ObtenerListaSolicitudPagoXPlanPagoIds(planPagoIds);

            //Obtener dededucciones de terceros
            listaDeduccionesXTercero = await ObtenerListaDeduccionesXTerceroIds(terceroIds, pciId);

            //Obtener lista de detalle liquidacion mes anterior viaticos SI
            listaDetalleLiquidacionMesAnteriorViatico = await ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(terceroIds, pciId);

            //Obtener lista de detalle liquidacion mes anterior viaticos NO
            listaDetalleLiquidacionMesAnterior = ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(terceroIds, pciId);

            #endregion Obtener listas secundarias

            //Proceso de obtención de detalles de liquidación
            listaDetalleLiquidacionARegistrar = ObtenerListaDetalleLiquidacionARegistrar(usuarioId,
                                                                                listaPlanPago,
                                                                                listaSolicitudPago,
                                                                                listaDetallePlanPago,
                                                                                listaParametroLiquidacionTercero,
                                                                                listaDeduccionesXTercero,
                                                                                listaDetalleLiquidacionMesAnteriorViatico,
                                                                                listaDetalleLiquidacionMesAnterior);

            #region Proceso de Actualización de BD

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            var bulkConfig = new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true };

            #region Establecer orden de inserccion

            var list = listaDetalleLiquidacionARegistrar.ToList();
            int i = -list.Count();
            foreach (var item in list)
            {
                item.DetalleLiquidacionId = i++;
            }

            #endregion Establecer orden de inserccion

            //Insertar liquidación 
            await _dataContext.BulkInsertAsync(listaDetalleLiquidacionARegistrar, bulkConfig);

            //Inserta deducciones de liquidación
            foreach (var detalleLiquidacion in listaDetalleLiquidacionARegistrar)
            {
                foreach (var deduccion in detalleLiquidacion.Deducciones)
                {
                    deduccion.DetalleLiquidacionId = detalleLiquidacion.DetalleLiquidacionId; // setting FK to match its linked PK that was generated in DB
                }
                listaLiquidacionDeduccionARegistrar.AddRange(detalleLiquidacion.Deducciones);
            }

            await _dataContext.BulkInsertAsync(listaLiquidacionDeduccionARegistrar);

            //Actualización de Plan
            var q = from pp in _dataContext.PlanPago
                    where planPagoIds.Contains(pp.PlanPagoId)
                    select pp;

            int affected = q.BatchUpdate(new PlanPago
            {
                EstadoPlanPagoId = planPagoEstado_ConLiquidacionDeducciones,
                UsuarioIdModificacion = usuarioId,
                FechaModificacion = _generalInterface.ObtenerFechaHoraActual(),
            });
            _dataContext.SaveChanges();

            await transaction.CommitAsync();

            #endregion Proceso de Actualización de BD

        }

        private void ActualizarListaDetallePlanPago(List<DetallePlanPagoDto> listaDetallePlanPago, List<DetallePlanPagoDto> listaCantidadMaximaXPlanPago)
        {
            foreach (var planPago in listaDetallePlanPago)
            {
                var item = listaCantidadMaximaXPlanPago.Where(x => x.Crp == planPago.Crp).FirstOrDefault();
                planPago.CantidadPago = item.CantidadPago;
            }
        }

        private List<PlanPago> FiltrarListaPlanesPago(IEnumerable<PlanPago> listaPlanPagoTotal)
        {
            //Filtrar Proveedor con deducción y Tipo Pago Variable
            var listaPlanPagoFiltro = (from l in listaPlanPagoTotal
                                       where l.Tercero.TipoPago != (int)TipoPago.Variable
                                       where l.Tercero.ModalidadContrato != (int)ModalidadContrato.ProveedorConDescuento
                                       select l).ToList();

            return listaPlanPagoFiltro;
        }

        private List<TerceroDeduccion> FiltrarListaDeduccionesXTerceroyActividadEconomica(List<TerceroDeduccion> listaTerceroDeduccion, FormatoSolicitudPago solicitudPago)
        {
            List<TerceroDeduccion> lista = new List<TerceroDeduccion>();

            var listaDeduccion = (from td in listaTerceroDeduccion
                                  where td.TerceroId == solicitudPago.TerceroId
                                  where td.ActividadEconomicaId == solicitudPago.ActividadEconomicaId
                                  select td).ToList();

            if (listaDeduccion != null && listaDeduccion.Count > 0)
            {
                lista.AddRange(listaDeduccion);
            }

            return lista;
        }

        private async Task<List<DetallePlanPagoDto>> ObtenerListaCantidadMaximaPlanPago(List<long> compromisos, int pciId)
        {
            var lista = await _planPagoRepository.ObtenerListaCantidadMaximaPlanPago(compromisos, pciId);
            return lista.ToList();
        }

        private async Task<List<ParametroLiquidacionTercero>> ObtenerListaParametroLiquidacionTerceroXIds(List<int> terceroIds, int pciId)
        {
            var listaParametroLiquidacion = await _terceroRepository.ObtenerListaParametroLiquidacionTerceroXIds(terceroIds, pciId);
            return listaParametroLiquidacion.ToList();
        }

        private async Task<List<TerceroDeduccion>> ObtenerListaDeduccionesXTerceroIds(List<int> terceroIds, int pciId)
        {
            var listaDeducciones = await _terceroRepository.ObtenerListaDeduccionesXTerceroIds(terceroIds, pciId);
            return listaDeducciones.ToList();
        }

        private async Task<List<FormatoSolicitudPago>> ObtenerListaSolicitudPagoXPlanPagoIds(List<int> planPagoIds)
        {
            var lista = await _solicitudPagoRepository.ObtenerListaSolicitudPagoXPlanPagoIds(planPagoIds);
            return lista.ToList();
        }

        private async Task<List<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(List<int> terceroIds, int pciId)
        {
            var lista = await _repo.ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(terceroIds, pciId);
            return lista.ToList();
        }

        private List<DetalleLiquidacion> ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(List<int> terceroIds, int pciId)
        {
            var lista = _repo.ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(terceroIds, pciId);
            return lista.ToList();
        }

        private async Task<List<DetallePlanPagoDto>> ObtenerListaDetallePlanPagoXIds(List<int> listaPlanPagoIds)
        {
            var lista = await _planPagoRepository.ObtenerListaDetallePlanPagoXIds(listaPlanPagoIds);
            return lista.ToList();
        }

        private void MapearFormatoLiquidacionPlanPago(DetallePlanPagoDto detallePlanPago, DetalleLiquidacion detalleLiquidacion)
        {
            try
            {
                detalleLiquidacion.PlanPagoId = detallePlanPago.PlanPagoId;

                detalleLiquidacion.NumeroIdentificacion = detallePlanPago.IdentificacionTercero;
                detalleLiquidacion.TerceroId = detallePlanPago.TerceroId;
                detalleLiquidacion.Nombre = detallePlanPago.NombreTercero;
                detalleLiquidacion.Contrato = detallePlanPago.Detalle6;
                detalleLiquidacion.Viaticos = detallePlanPago.ViaticosDescripcion;
                detalleLiquidacion.Crp = detallePlanPago.Crp;
                detalleLiquidacion.CantidadPago = detallePlanPago.CantidadPago;
                detalleLiquidacion.NumeroPago = detallePlanPago.NumeroPago;

                detalleLiquidacion.ValorContrato = detallePlanPago.ValorTotal;
                detalleLiquidacion.ValorAdicionReduccion = detallePlanPago.Operacion;
                detalleLiquidacion.ValorCancelado = detallePlanPago.ValorTotal - detallePlanPago.SaldoActual;
                detalleLiquidacion.TotalACancelar = (detallePlanPago.ValorFacturado.HasValue ? detallePlanPago.ValorFacturado.Value : 0);
                detalleLiquidacion.SaldoActual = (detallePlanPago.SaldoActual - (detallePlanPago.ValorFacturado.HasValue ? detallePlanPago.ValorFacturado.Value : 0));
                detalleLiquidacion.RubroPresupuestal = string.Empty;
                detalleLiquidacion.UsoPresupuestal = string.Empty;

                detalleLiquidacion.NombreSupervisor = detallePlanPago.Detalle5;
                detalleLiquidacion.NumeroRadicado = detallePlanPago.NumeroRadicadoSupervisor;
                detalleLiquidacion.FechaRadicado = detallePlanPago.FechaRadicadoSupervisor.Value;
                detalleLiquidacion.NumeroFactura = detallePlanPago.NumeroFactura;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MapearFormatoLiquidacion(FormatoCausacionyLiquidacionPagos formato, DetalleLiquidacion detalleLiquidacion)
        {
            try
            {
                //detalleLiquidacion.CantidadPago = formato.CantidadPago;
                detalleLiquidacion.TextoComprobanteContable = EliminarCaracteresEspeciales(formato.TextoComprobanteContable);

                detalleLiquidacion.ViaticosPagados = formato.ViaticosPagados;
                detalleLiquidacion.Honorario = formato.Honorario;
                detalleLiquidacion.HonorarioUvt = formato.HonorarioUvt;
                detalleLiquidacion.ValorIva = formato.ValorIva;
                detalleLiquidacion.ValorTotal = formato.ValorTotal;
                detalleLiquidacion.TotalRetenciones = formato.TotalRetenciones;
                detalleLiquidacion.TotalAGirar = formato.TotalAGirar;

                detalleLiquidacion.BaseSalud = formato.BaseSalud;
                detalleLiquidacion.AporteSalud = formato.AporteSalud;
                detalleLiquidacion.AportePension = formato.AportePension;
                detalleLiquidacion.RiesgoLaboral = formato.RiesgoLaboral;
                detalleLiquidacion.FondoSolidaridad = formato.FondoSolidaridad;
                detalleLiquidacion.ImpuestoCovid = formato.ImpuestoCovid;
                detalleLiquidacion.SubTotal1 = formato.SubTotal1;

                detalleLiquidacion.PensionVoluntaria = formato.PensionVoluntaria;
                detalleLiquidacion.Afc = formato.Afc;
                detalleLiquidacion.SubTotal2 = formato.SubTotal2;
                detalleLiquidacion.MedicinaPrepagada = formato.MedicinaPrepagada;
                detalleLiquidacion.Dependientes = formato.Dependientes;
                detalleLiquidacion.InteresesVivienda = formato.InteresVivienda;
                detalleLiquidacion.TotalDeducciones = formato.TotalDeducciones;

                detalleLiquidacion.SubTotal3 = formato.SubTotal3;
                detalleLiquidacion.RentaExenta = formato.RentaExenta;
                detalleLiquidacion.LimiteRentaExenta = formato.LimiteRentaExenta;
                detalleLiquidacion.TotalRentaExenta = formato.TotalRentaExenta;
                detalleLiquidacion.DiferencialRenta = formato.DiferencialRenta;
                detalleLiquidacion.BaseGravableRenta = formato.BaseGravableRenta;
                detalleLiquidacion.BaseGravableUvt = formato.BaseGravableUvt;

                detalleLiquidacion.ModalidadContrato = formato.ModalidadContrato;
                detalleLiquidacion.MesSaludAnterior = formato.NumeroMesSaludAnterior;
                detalleLiquidacion.MesSaludActual = formato.NumeroMesSaludActual;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string EliminarCaracteresEspeciales(string texto)
        {
            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            stringBuilder.Replace("ñ", "n");

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void MapearLiquidacionDeducciones(FormatoCausacionyLiquidacionPagos formato, DetalleLiquidacion detalleLiquidacion)
        {
            LiquidacionDeduccion liquidacionDeduccion = null;
            detalleLiquidacion.Deducciones = new List<LiquidacionDeduccion>();

            if (formato.Deducciones != null && formato.Deducciones.Count > 0)
            {
                foreach (var deduccion in formato.Deducciones)
                {
                    liquidacionDeduccion = new LiquidacionDeduccion();
                    liquidacionDeduccion.DeduccionId = deduccion.DeduccionId;
                    liquidacionDeduccion.Codigo = deduccion.Codigo;
                    liquidacionDeduccion.Nombre = deduccion.Nombre;
                    liquidacionDeduccion.Tarifa = deduccion.Tarifa;
                    liquidacionDeduccion.Base = deduccion.Base;
                    liquidacionDeduccion.Valor = deduccion.Valor;
                    detalleLiquidacion.Deducciones.Add(liquidacionDeduccion);
                }
            }
        }

        #endregion Liquidación Masiva

        private bool FechaActualEntreFechasVigencia(DateTime fechaInicio, DateTime fechaFinal)
        {
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();
            if (fechaInicio < fechaActual && fechaActual < fechaFinal)
            {
                return true;
            }
            return false;
        }

        private List<ParametroGeneral> obtenerParametrosGeneralesXTipo(List<ParametroGeneral> parametroGenerales, string nombre)
        {
            List<ParametroGeneral> parametros = new List<ParametroGeneral>();
            var items = parametroGenerales.Where(x => x.Nombre.ToUpper() == nombre.ToUpper()).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    parametros.Add(item);
                }
            }

            return parametros;
        }

        private bool DeduccionEsParametroGeneral(List<ParametroGeneral> parametroGenerales, string codigoDeduccion)
        {
            bool resultado = false;
            resultado = parametroGenerales.Exists(p => p.Valor == codigoDeduccion);
            return resultado;
        }

        private string ObtenerValorDeParametroGeneral(List<ParametroGeneral> parametroGenerales, string nombre)
        {
            string valor = string.Empty;
            var item = parametroGenerales.FirstOrDefault(x => x.Nombre.ToUpper() == nombre.ToUpper());

            if (item != null)
                valor = item.Valor;

            return valor;
        }

        private CriterioCalculoReteFuente ObtenerCriterioCalculoRendimiento(List<CriterioCalculoReteFuente> lista, decimal baseGravableUvtFinal)
        {
            var criterio = lista.Where(x => x.Desde <= baseGravableUvtFinal
                                    && baseGravableUvtFinal <= x.Hasta).FirstOrDefault();

            return criterio;
        }


        private decimal ObtenerValorViaticosAnteriores(List<DetalleLiquidacion> listaDetalleLiquidacion)
        {
            decimal valor = 0;
            foreach (var item in listaDetalleLiquidacion)
            {
                //Base salud para viaticos no cuenta
                //if (item.BaseImpuestos.HasValue && !item.BaseImpuestos.Value)
                {
                    valor = valor + item.TotalACancelar;
                }
            }
            return valor;
        }


        #endregion Funciones Generales
    }
}
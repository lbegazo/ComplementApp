using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Services
{
    public class ProcesoLiquidacionPlanPago : IProcesoLiquidacionPlanPago
    {
        #region Constantes

        const string codigoRenta = "CodigoRenta";
        const string codigoIva = "CodigoIva";
        const string valorUVT = "ValorUVT";
        const string salarioMinimo = "SalarioMinimo";
        const string codigoPensionVoluntaria = "CodigoPensionVoluntaria";
        const string codigoAFC = "CodigoAFC";

        #endregion Constantes

        private readonly IPlanPagoRepository _repo;
        private readonly IListaRepository _repoLista;
        private readonly IMapper _mapper;

        public ProcesoLiquidacionPlanPago(IPlanPagoRepository repo,
                                            IListaRepository listaRepository, IMapper mapper)
        {
            this._repo = repo;
            this._repoLista = listaRepository;
            this._mapper = mapper;
        }
        public async Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacionyLiquidacionPago(int planPagoId,
                                                                                                        decimal valorBaseGravable)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {
                var planPagoDto = await _repo.ObtenerDetallePlanPago(planPagoId);

                IEnumerable<ParametroGeneral> parametroGenerales = await _repoLista.ObtenerParametrosGenerales();
                var parametros = parametroGenerales.ToList();

                ParametroLiquidacionTercero parametroLiquidacion = await _repoLista.ObtenerParametroLiquidacionXTercero(planPagoDto.TerceroId);

                ICollection<Deduccion> listaDeducciones = await _repo.ObtenerDeduccionesXTercero(planPagoDto.TerceroId);
                var listaDeduccionesDto = _mapper.Map<ICollection<DeduccionDto>>(listaDeducciones);

                ICollection<CriterioCalculoReteFuente> listaCriterioReteFuente = await _repoLista.ObtenerListaCriterioCalculoReteFuente();

                if (parametroGenerales != null && parametroLiquidacion != null && listaCriterioReteFuente != null)
                {
                    if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ContratoPrestacionServicio)
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
                            formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto, parametroLiquidacion,
                                                                                        parametros, listaCriterioReteFuente.ToList());
                        }
                    }
                    else if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ProveedorConDescuento)
                    {
                        if (parametroLiquidacion.TipoPago == (int)TipoPago.Fijo)
                        {
                            formato = ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(planPagoDto, parametroLiquidacion,
                                                                                                    parametros, listaCriterioReteFuente.ToList(),
                                                                                                    listaDeduccionesDto.ToList());
                        }
                        else
                        {
                            if (valorBaseGravable < 0)
                            {
                                throw new Exception($"No se registró el valor base gravable");
                            }

                            formato = ObtenerFormatoCausacion_ProveedoresConDeduccionVariable(planPagoDto, parametroLiquidacion,
                                                                        parametros, listaCriterioReteFuente.ToList(),
                                                                        listaDeduccionesDto.ToList(), valorBaseGravable);
                        }

                    }
                    else if (parametroLiquidacion.ModalidadContrato == (int)ModalidadContrato.ProveedorSinDescuento)
                    {
                        formato = ObtenerFormatoCausacion_ProveedoresSinDeduccion(planPagoDto, parametroLiquidacion,
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
            FormatoCausacionyLiquidacionPagos formatoIgual30 = new FormatoCausacionyLiquidacionPagos();
            CriterioCalculoReteFuente criterioReteFuente = null;

            decimal C30ValorIva = 0, PL17HonorarioSinIva = 0, baseGravable30 = 0, baseGravableFinal = 0, valorUvt, baseGravableUvtCalculada = 0;
            int C32NumeroDiaLaborados = 0, baseGravableUvtFinal = 0;

            decimal valor = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PL17HonorarioSinIva = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;

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
                valor = (((planPago.ValorFacturado.Value / (1 + tarifaIva)) * 30) / PL17HonorarioSinIva);
                C32NumeroDiaLaborados = (int)Math.Round(valor, 0, MidpointRounding.AwayFromZero);
            }

            #endregion Numero de dias laborados

            #region Calcular valores y obtener formato

            if (C32NumeroDiaLaborados < 30)
            {
                formato = await CalcularValoresFormatoContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: C32NumeroDiaLaborados);
                formatoIgual30 = await CalcularValoresFormatoContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: 30);

                baseGravable30 = formatoIgual30.BaseGravableRenta;
                baseGravableFinal = (baseGravable30 / 30) * C32NumeroDiaLaborados;
                baseGravableUvtCalculada = baseGravable30 / valorUvt;
                baseGravableUvtFinal = (int)Math.Round(baseGravableUvtCalculada, 0, MidpointRounding.AwayFromZero);

                formato.BaseGravableRenta = baseGravableFinal;
                formato.BaseGravableUvt = baseGravableUvtFinal;
            }
            else
            {
                formatoIgual30 = await CalcularValoresFormatoContratoPrestacionServicio(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: 30);
                formato = formatoIgual30;
                baseGravableUvtCalculada = formato.BaseGravableUvtCalculada;
                baseGravableFinal = formatoIgual30.BaseGravableRenta;
                baseGravableUvtFinal = formatoIgual30.BaseGravableUvt;
            }

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
                        var valorRentaCalculado = (((((tarifaCalculo / 100) * (baseGravableUvtCalculada - valorMinimoRango))
                                                        + factorIncremento) * valorUvt) / 30) * C32NumeroDiaLaborados;
                        deduccion.Valor = ObtenerValorRedondeadoAl1000XEncima(valorRentaCalculado);

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
                                            List<ParametroGeneral> parametroGenerales, int numeroDiasLaborados)
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
            PL13PensionVoluntaria = 0, PL14Afc = 0, PL16MedicinaPrepagada = 0,
            PL17HonorarioSinIva = 0, PL17DescuentoDependiente = 0,
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
            PL13PensionVoluntaria = parametroLiquidacion.PensionVoluntaria;
            PL14Afc = parametroLiquidacion.Afc;
            PL16MedicinaPrepagada = parametroLiquidacion.MedicinaPrepagada;
            PL17HonorarioSinIva = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;
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

            #region Viaticos

            var listaDetalleLiquidacion = await _repo.ObtenerListaDetalleLiquidacionAnterior(planPago.TerceroId);

            if (listaDetalleLiquidacion != null && listaDetalleLiquidacion.Count > 0)
            {
                viaticosPagados = ObtenerValorViaticosAnteriores(listaDetalleLiquidacion.ToList());
            }

            #endregion Viáticos

            #region Honorario 

            if (numeroDiasLaborados < 30)
            {
                C1honorario = (planPago.ValorFacturado.Value + viaticosPagados) / (1 + PLtarifaIva);
            }
            else
            {
                C1honorario = PL17HonorarioSinIva + viaticosPagados;
            }

            #endregion Honorario 

            if (decimal.TryParse(parametroUvt, out valorUvt))
            {
                if (valorUvt > 0)
                {
                    C2honorarioUvt = C1honorario / valorUvt;
                    C2honorarioUvtFinal = (int)Math.Round(C2honorarioUvt, 0, MidpointRounding.AwayFromZero);
                }
            }

            C3valorIva = C1honorario * (PLtarifaIva);
            C7baseAporteSalud = C1honorario * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C8aporteASalud = ObtenerValorRedondeadoAl100XEncima(C8aporteASalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C9aporteAPension = ObtenerValorRedondeadoAl100XEncima(C9aporteAPension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);
            C10aporteRiesgoLaboral = ObtenerValorRedondeadoAl100XEncima(C10aporteRiesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                cuatroSMLV = 4 * valorSMLV;
            }

            #region Fondo de solidaridad

            if (C7baseAporteSalud > cuatroSMLV)
            {
                C11fondoSolidaridad = C7baseAporteSalud * (PLfondoSolidaridad);
                C11fondoSolidaridad = ObtenerValorRedondeadoAl100XEncima(C11fondoSolidaridad);
            }
            else
            {
                C11fondoSolidaridad = 0;
            }

            #endregion Fondo de solidaridad

            C12subTotal1 = C1honorario - C8aporteASalud - C9aporteAPension - C10aporteRiesgoLaboral - C11fondoSolidaridad;
            C15subTotal2 = C12subTotal1 - PL13PensionVoluntaria - PL14Afc;

            #region Descuento Dependiente

            if (PL17HonorarioSinIva * valorDescuentoDependiente > valorUvt * 32)
            {
                C17DescuentoDependiente = valorUvt * 32;
            }
            else
            {
                C17DescuentoDependiente = PL17HonorarioSinIva * PL17DescuentoDependiente;
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

            C25BaseGravableRenta = ((C20subTotal3 - C21RentaExenta - C24DiferencialRenta) / 30) * numeroDiasLaborados;

            C26BaseGravableRentaUvt = C25BaseGravableRenta / valorUvt;
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
            formato.PensionVoluntaria = PL13PensionVoluntaria;
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

        #endregion Contrato Prestación de Servicio

        #region Proveedores con deducción

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresConDeduccionFijo(DetallePlanPagoDto planPago,
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

            C31Honorario = planPago.ValorFacturado.Value / (1 + PLTarifaIva);
            C30ValorIva = planPago.ValorFacturado.Value - C31Honorario;

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
            formato.ValorTotal = planPago.ValorFacturado.Value;
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
                        deduccion.Valor = ObtenerValorRedondeadoAl1000XEncima(deduccion.Valor);

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


        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresConDeduccionVariable(DetallePlanPagoDto planPago,
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

            C30ValorIva = valorBaseGravable * PLTarifaIva;
            C31Honorario = planPago.ValorFacturado.Value - C30ValorIva;
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
            formato.ValorTotal = planPago.ValorFacturado.Value;

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
                        deduccion.Valor = ObtenerValorRedondeadoAl1000XEncima(deduccion.Valor);
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

        #endregion Proveedores con deducción

        #region Proveeedores sin Deducción

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacion_ProveedoresSinDeduccion(DetallePlanPagoDto planPago,
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

            C31Honorario = (planPago.ValorFacturado.Value) / (1 + PLTarifaIva);
            C30ValorIva = planPago.ValorFacturado.Value - C31Honorario;

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
            formato.ValorTotal = planPago.ValorFacturado.Value;

            #endregion Calcular valores y obtener formato

            formato.TotalRetenciones = 0;
            formato.TotalAGirar = formato.ValorTotal;

            return formato;
        }


        #endregion Proveedores sin deducción

        #region Funciones Generales

        private bool FechaActualEntreFechasVigencia(DateTime fechaInicio, DateTime fechaFinal)
        {
            DateTime fechaActual = DateTime.Now;
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

        private decimal ObtenerValorRentaRedondeado(decimal valorRentaCalculado)
        {

            var modValorRentaCalculado = valorRentaCalculado % 100;

            if (modValorRentaCalculado < 50)
            {
                valorRentaCalculado = valorRentaCalculado - modValorRentaCalculado;
            }
            else
            {
                valorRentaCalculado = valorRentaCalculado + (100 - modValorRentaCalculado);
            }
            return valorRentaCalculado;
        }

        private decimal ObtenerValorRedondeadoAl100XEncima(decimal valor)
        {
            decimal valorNuevo = 0;
            var modValorRentaCalculado = valor % 100;

            if (modValorRentaCalculado > 0)
            {
                valorNuevo = valor + (100 - modValorRentaCalculado);
            }
            else
            {
                valorNuevo = valor;
            }

            return valorNuevo;
        }

        private decimal ObtenerValorRedondeadoAl1000XEncima(decimal valor)
        {
            decimal valorNuevo = 0;
            var modValorRentaCalculado = valor % 1000;

            if (modValorRentaCalculado > 0)
            {
                valorNuevo = valor + (1000 - modValorRentaCalculado);
            }
            else
            {
                valorNuevo = valor;
            }

            return valorNuevo;
        }

        private decimal ObtenerValorViaticosAnteriores(List<DetalleLiquidacion> listaDetalleLiquidacion)
        {
            decimal valor = 0;
            foreach (var item in listaDetalleLiquidacion)
            {
                if (item.BaseImpuestos.HasValue && !item.BaseImpuestos.Value)
                {
                    valor = +item.TotalACancelar;
                }
            }
            return valor;
        }

        #endregion Funciones Generales
    }
}
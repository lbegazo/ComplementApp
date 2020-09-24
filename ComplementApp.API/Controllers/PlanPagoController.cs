using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanPagoController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanPagoRepository _repo;
        private readonly IListaRepository _repoLista;

        private readonly IMapper _mapper;
        public PlanPagoController(IUnitOfWork unitOfWork, IPlanPagoRepository repo, IMapper mapper, DataContext dataContext, IListaRepository listaRepository)
        {
            this._repoLista = listaRepository;
            this._mapper = mapper;
            this._repo = repo;
            this._unitOfWork = unitOfWork;
            _dataContext = dataContext;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanPago([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery] UserParams userParams)
        {
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();

            var pagedList = await _repo.ObtenerListaPlanPago(terceroId, listIds, userParams);
            var listaDto = _mapper.Map<IEnumerable<PlanPagoDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPlanPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerPlanPago(planPagoId);
            var planPagoDto = _mapper.Map<PlanPagoDto>(planPagoBD);
            return base.Ok(planPagoDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPlanPagoDetallado([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerPlanPagoDetallado(planPagoId);
            //var planPagoDto = _mapper.Map<PlanPagoDto>(planPagoBD);
            return base.Ok(planPagoBD);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePlanPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerDetallePlanPago(planPagoId);
            //var planPagoDto = _mapper.Map<PlanPagoDto>(planPagoBD);
            return base.Ok(planPagoBD);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoCausacionyLiquidacionPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {

                var planPagoBD = await _repo.ObtenerDetallePlanPago(planPagoId);

                IEnumerable<ParametroGeneral> parametroGenerales = await _repoLista.ObtenerParametrosGenerales();
                var parametros = parametroGenerales.ToList();

                ParametroLiquidacionTercero parametroLiquidacion = await _repoLista.ObtenerParametroLiquidacionXTercero(planPagoBD.TerceroId);

                ICollection<Deduccion> listaDeducciones = await _repo.ObtenerDeduccionesXTercero(planPagoBD.TerceroId);
                var listaDeduccionesDto = _mapper.Map<ICollection<DeduccionDto>>(listaDeducciones);

                ICollection<CriterioCalculoReteFuente> listaCriterioReteFuente = await _repoLista.ObtenerListaCriterioCalculoReteFuente();

                if (parametroGenerales != null && parametroLiquidacion != null && listaCriterioReteFuente != null)
                {
                    formato = ObtenerFormatoCausacionCalculado(planPagoBD, parametroLiquidacion,
                                                                parametros, listaCriterioReteFuente.ToList(),
                                                                listaDeduccionesDto.ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return base.Ok(formato);
        }


        [HttpPut]
        public async Task<IActionResult> ActualizarPlanPago(PlanPagoDto planPagoDto)
        {
            //PlanPagoDto planPagoDto = null;
            if (planPagoDto != null)
            {
                //Obtener todos los campos del plan de pago para una correcta actualización
                var planPagoBD = await _repo.ObtenerPlanPago(planPagoDto.PlanPagoId);

                _mapper.Map(planPagoDto, planPagoBD);

                if (planPagoDto.esRadicarFactura)
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = System.DateTime.Now;
                }
                else
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = System.DateTime.Now;
                }

                //Para apagar manualmente la columna que no deseo modificar
                // _dataContext.Entry(planPagoBD).Property("RubroPresupuestalId").IsModified = false;

                //await _unitOfWork.CompleteAsync();
                //Para solicitar la actualización de una entidad
                //_dataContext.Update(planPagoBD);
                await _unitOfWork.CompleteAsync();
                return NoContent();
            }

            throw new Exception($"No se pudo actualizar la factura");
        }

        private FormatoCausacionyLiquidacionPagos ObtenerFormatoCausacionCalculado(DetallePlanPagoDto planPago,
                                            ParametroLiquidacionTercero parametroLiquidacion,
                                            List<ParametroGeneral> parametroGenerales,
                                            List<CriterioCalculoReteFuente> listaCriterioReteFuente,
                                            List<DeduccionDto> listaDeducciones)
        {
            #region variables 

            FormatoCausacionyLiquidacionPagos formato = new FormatoCausacionyLiquidacionPagos();
            FormatoCausacionyLiquidacionPagos formatoIgual30 = new FormatoCausacionyLiquidacionPagos();
            CriterioCalculoReteFuente criterioReteFuente = null;

            decimal PL17HonorarioSinIva = 0, baseGravable30 = 0, baseGravableFinal = 0, valorUvt;
            int C32NumeroDiaLaborados = 0, baseGravableUvtFinal = 0;

            #endregion variables 

            #region Parametros de liquidación de tercero

            PL17HonorarioSinIva = parametroLiquidacion.HonorarioSinIva.HasValue ? parametroLiquidacion.HonorarioSinIva.Value : 0;

            #endregion Parametros de liquidación de tercero

            #region Parametros Generales

            var parametroUvt = obtenerValorDeParametroGeneral(parametroGenerales, "ValorUVT");
            var parametroSMLV = obtenerValorDeParametroGeneral(parametroGenerales, "SalarioMinimo");
            var parametroCodigoRenta = obtenerValorDeParametroGeneral(parametroGenerales, "CodigoRenta ");
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");
            decimal.TryParse(parametroUvt, out valorUvt);

            #endregion Parametros Generales

            #region Numero de dias laborados

            if (PL17HonorarioSinIva > 0)
            {
                C32NumeroDiaLaborados = (int)((planPago.ValorFacturado.Value * 30) / PL17HonorarioSinIva);
            }

            #endregion Numero de dias laborados

            #region Calcular valores y obtener formato

            if (C32NumeroDiaLaborados < 30)
            {
                formato = CalcularValoresFormato(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: C32NumeroDiaLaborados);
                formatoIgual30 = CalcularValoresFormato(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: 30);

                baseGravable30 = formatoIgual30.BaseGravableRenta;
                baseGravableFinal = (baseGravable30 / 30) * C32NumeroDiaLaborados;
                baseGravable30 = baseGravable30 / valorUvt;
                baseGravableUvtFinal = (int)Math.Round(baseGravable30, 0, MidpointRounding.AwayFromZero);

                formato.BaseGravableRenta = baseGravableFinal;
                formato.BaseGravableUvt = baseGravableUvtFinal;
            }
            else
            {
                formatoIgual30 = CalcularValoresFormato(planPago, parametroLiquidacion, parametroGenerales, numeroDiasLaborados: 30);
                formato = formatoIgual30;
                baseGravableFinal = formatoIgual30.BaseGravableRenta;
                baseGravableUvtFinal = formatoIgual30.BaseGravableUvt;
            }

            #endregion Calcular valores y obtener formato

            #region Obtener Lista de deducciones

            string deduccionRentaTrabajo = parametroCodigoRenta;

            decimal valorMinimoRango = 0;
            decimal factorIncremento = 0;
            decimal tarifaCalculo = 0;

            criterioReteFuente = obtenerCriterioCalculoRendimiento(listaCriterioReteFuente, baseGravableUvtFinal);

            valorMinimoRango = criterioReteFuente.Desde;
            factorIncremento = criterioReteFuente.Factor;
            tarifaCalculo = criterioReteFuente.Tarifa;

            if (listaDeducciones != null && listaDeducciones.Count > 0)
            {
                foreach (var deduccion in listaDeducciones)
                {
                    if (deduccion.Codigo.Equals(deduccionRentaTrabajo))
                    {
                        deduccion.Base = baseGravableFinal;
                        deduccion.Valor = ((tarifaCalculo / 100) * (baseGravableUvtFinal - valorMinimoRango + factorIncremento) * valorUvt / 30) * C32NumeroDiaLaborados;

                        //if (deduccion.Base > 0)
                        {
                            deduccion.Tarifa = deduccion.Valor / deduccion.Base;
                        }
                    }

                    if (!deduccion.Codigo.Equals(deduccionRentaTrabajo) &&
                        (deduccion.TipoBaseDeduccionId != (int)TipoBaseDeducciones.OTRAS))
                    {
                        deduccion.Base = formato.SubTotal1;
                        deduccion.Valor = deduccion.Tarifa * formato.SubTotal1;
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
                deduccionOtras.Valor = deduccionOtras.Tarifa * valorGmf;
            }

            #endregion Deducción Otras


            formato.Deducciones = new List<DeduccionDto>();
            formato.Deducciones = listaDeducciones;

            #endregion Obtener lista de deducciones          

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


        private FormatoCausacionyLiquidacionPagos CalcularValoresFormato(DetallePlanPagoDto planPago, ParametroLiquidacionTercero parametroLiquidacion, List<ParametroGeneral> parametroGenerales, int numeroDiasLaborados)
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
            PL18InteresVivienda = 0, PL24PensionVoluntaria = 0;

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

            var parametroUvt = obtenerValorDeParametroGeneral(parametroGenerales, "ValorUVT");
            var parametroSMLV = obtenerValorDeParametroGeneral(parametroGenerales, "SalarioMinimo");
            parametroUvt = parametroUvt.Replace(",", "");
            parametroSMLV = parametroSMLV.Replace(",", "");

            #endregion Parametros Generales

            #region Honorario 

            if (numeroDiasLaborados < 30)
            {
                C1honorario = planPago.ValorFacturado.Value / (1 + PLtarifaIva);
            }
            else
            {
                C1honorario = PL17HonorarioSinIva;
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

            C3valorIva = planPago.ValorFacturado.Value - C1honorario;
            C7baseAporteSalud = C1honorario * PLbaseAporteSalud;
            C8aporteASalud = C7baseAporteSalud * (PLaporteSalud);
            C9aporteAPension = C7baseAporteSalud * (PLaportePension);
            C10aporteRiesgoLaboral = C7baseAporteSalud * (PLriesgoLaboral);

            if (decimal.TryParse(parametroSMLV, out valorSMLV))
            {
                cuatroSMLV = 4 * valorSMLV;
            }

            #region Fondo de solidaridad

            if (C7baseAporteSalud > cuatroSMLV)
            {
                C11fondoSolidaridad = C7baseAporteSalud * (PLfondoSolidaridad);
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

            //if (numeroDiasLaborados < 30)
            {
                C25BaseGravableRenta = ((C20subTotal3 - C21RentaExenta - C24DiferencialRenta) / 30) * numeroDiasLaborados;
            }
            // else
            // {
            //     C25BaseGravableRenta = C20subTotal3 - C21RentaExenta - C24DiferencialRenta;
            // }
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
            formato.TotalDeducciones = C19TotalDeducciones;
            formato.DiferencialRenta = C24DiferencialRenta;
            formato.BaseGravableRenta = C25BaseGravableRenta;
            formato.BaseGravableUvt = C26BaseGravableRentaUvtFinal;

            #endregion Setear valores a formato

            return formato;
        }

        private string obtenerValorDeParametroGeneral(List<ParametroGeneral> parametroGenerales, string nombre)
        {
            string valor = string.Empty;
            var item = parametroGenerales.FirstOrDefault(x => x.Nombre.ToUpper() == nombre.ToUpper());

            if (item != null)
                valor = item.Valor;

            return valor;
        }


        private bool FechaActualEntreFechasVigencia(DateTime fechaInicio, DateTime fechaFinal)
        {
            DateTime fechaActual = DateTime.Now;
            if (fechaInicio < fechaActual && fechaActual < fechaFinal)
            {
                return true;
            }
            return false;
        }

        private CriterioCalculoReteFuente obtenerCriterioCalculoRendimiento(List<CriterioCalculoReteFuente> lista, int baseGravableUvtFinal)
        {
            var criterio = lista.Where(x => x.Desde <= baseGravableUvtFinal
                                    && baseGravableUvtFinal <= x.Hasta).FirstOrDefault();

            return criterio;
        }
    }
}
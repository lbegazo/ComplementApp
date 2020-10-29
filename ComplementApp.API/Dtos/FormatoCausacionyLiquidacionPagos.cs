using System;
using System.Collections.Generic;

namespace ComplementApp.API.Dtos
{
    public class FormatoCausacionyLiquidacionPagos
    {
        public int DetalleLiquidacionId { get; set; }
        public int PlanPagoId { get; set; }
        public int TerceroId { get; set; }

        #region Plan de Pago
        public string IdentificacionTercero { get; set; }
        public string NombreTercero { get; set; }
        public string Contrato { get; set; }
        public bool Viaticos { get; set; }
        public string ViaticosDescripcion { get; set; }
        public string Crp { get; set; }
        public int CantidadPago { get; set; }
        public int NumeroPago { get; set; }
        public int ModalidadContrato { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal ValorAdicionReduccion { get; set; }
        public decimal ValorCancelado { get; set; }
        public decimal TotalACancelar { get; set; }
        public decimal SaldoActual { get; set; }
        public string IdentificacionRubroPresupuestal { get; set; }
        public string IdentificacionUsoPresupuestal { get; set; }
        public string NombreSupervisor { get; set; }
        public string NumeroRadicadoSupervisor { get; set; }
        public DateTime FechaRadicadoSupervisor { get; set; }
        public string NumeroFactura { get; set; }
        public string TextoComprobanteContable { get; set; }
        public decimal ViaticosPagados { get; set; }
        public int NumeroMesSaludAnterior { get; set; }
        public int NumeroMesSaludActual { get; set; }
        public string MesSaludAnterior { get; set; }
        public string MesSaludActual { get; set; }

        #endregion Plan de Pago

        #region Formato de liquidación
        public decimal Honorario { get; set; }
        public int HonorarioUvt { get; set; }
        public decimal ValorIva { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal TotalRetenciones { get; set; }
        public decimal TotalAGirar { get; set; }

        public decimal BaseSalud { get; set; }
        public decimal AporteSalud { get; set; }
        public decimal AportePension { get; set; }
        public decimal RiesgoLaboral { get; set; }
        public decimal FondoSolidaridad { get; set; }
        public decimal ImpuestoCovid { get; set; }
        public decimal SubTotal1 { get; set; }

        public decimal PensionVoluntaria { get; set; }
        public decimal Afc { get; set; }
        public decimal SubTotal2 { get; set; }
        public decimal MedicinaPrepagada { get; set; }
        public decimal Dependientes { get; set; }
        public decimal InteresVivienda { get; set; }
        public decimal TotalDeducciones { get; set; }

        public decimal SubTotal3 { get; set; }
        public decimal RentaExenta { get; set; }
        public decimal LimiteRentaExenta { get; set; }
        public decimal TotalRentaExenta { get; set; }
        public decimal DiferencialRenta { get; set; }
        public decimal BaseGravableRenta { get; set; }
        public int BaseGravableUvt { get; set; }
        public decimal BaseGravableUvtCalculada { get; set; }

        public ICollection<DeduccionDto> Deducciones { get; set; }

        #endregion Formato de liquidación
    }
}
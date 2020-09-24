using System.Collections.Generic;

namespace ComplementApp.API.Dtos
{
    public class FormatoCausacionyLiquidacionPagos
    {
        public int PlanPagoId { get; set; }

        public int TerceroId { get; set; }

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

        public ICollection<DeduccionDto> Deducciones { get; set; }
    }
}
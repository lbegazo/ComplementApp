using System;

namespace ComplementApp.API.Dtos
{
    public class DetallePlanPago
    {
        public int PlanPagoId { get; set; }
        public string Detalle4 { get; set; }
        public string Detalle5 { get; set; }
        public string Detalle6 { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoActual { get; set; }
        public DateTime Fecha { get; set; }

    }
}
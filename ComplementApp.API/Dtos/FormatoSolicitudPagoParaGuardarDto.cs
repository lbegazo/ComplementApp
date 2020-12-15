using System;

namespace ComplementApp.API.Dtos
{
    public class FormatoSolicitudPagoParaGuardarDto
    {
        public int FormatoSolicitudPagoId { get; set; }
        public int TerceroId { get; set; }
        public int PlanPagoId { get; set; }
        public long Crp { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal valorFacturado { get; set; }
        public int MesId { get; set; }
        public int ActividadEconomicaId { get; set; }
        public ValorSeleccion ActividadEconomica { get; set; }
        public string NumeroPlanilla { get; set; }
        public string NumeroFactura { get; set; }
        public string Observaciones { get; set; }
        public decimal BaseCotizacion { get; set; }
    }
}
using System;

namespace ComplementApp.API.Dtos
{
    public class RadicadoDto
    {
        public int Consecutivo { get; set; }
        public int PlanPagoId { get; set; }
        public DateTime FechaRadicadoSupervisor { get; set; }
        public string FechaRadicadoSupervisorDescripcion { get; set; }
        public string Estado { get; set; }
        public string Crp { get; set; }
        public string NIT { get; set; }
        public string NombreTercero { get; set; }
        public string NumeroRadicadoProveedor { get; set; }
        public string NumeroRadicadoSupervisor { get; set; }
        public decimal ValorAPagar { get; set; }
        public string Obligacion { get; set; }
        public string OrdenPago { get; set; }
        public DateTime? FechaOrdenPago { get; set; }
        public string FechaOrdenPagoDescripcion { get; set; }
        public string TextoComprobanteContable { get; set; }

    }
}
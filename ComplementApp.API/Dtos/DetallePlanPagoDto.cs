using System;

namespace ComplementApp.API.Dtos
{
    public class DetallePlanPagoDto
    {
        public int PlanPagoId { get; set; }
        public string IdentificacionTercero { get; set; }
        public string NombreTercero { get; set; }
        public bool Viaticos { get; set; }
        public string ViaticosDescripcion { get; set; }
        public long Crp { get; set; }
        public int NumeroPago { get; set; }
        public int CantidadPago { get; set; }
        public string Observaciones { get; set; }

        public decimal? ValorFacturado { get; set; }
        public string IdentificacionRubroPresupuestal { get; set; }
        public string IdentificacionUsoPresupuestal { get; set; }
        public string NumeroRadicadoSupervisor { get; set; }
        public DateTime? FechaRadicadoSupervisor { get; set; }
        public string NumeroFactura { get; set; }

        public string Detalle4 { get; set; }
        public string Detalle5 { get; set; }
        public string Detalle6 { get; set; }
        public string Detalle7 { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal Operacion { get; set; }
        public DateTime Fecha { get; set; }
        public int TerceroId { get; set; }

        public int ModalidadContrato { get; set; }
        public int TipoPago { get; set; }
        public string NumeroRadicadoProveedor { get; set; }
        public DateTime? FechaRadicadoProveedor { get; set; }
        public string FechaRadicadoProveedorFormato { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public string FechaRadicadoSupervisorFormato { get; set; }
        public string FechaInicioSolicitudPagoFormato { get; set; }
        public string FechaFinalSolicitudPagoFormato { get; set; }
        public string TextoComprobanteContable { get; set; }
    }
}
using System;

namespace ComplementApp.API.Dtos
{
    public class CDPDto
    {
        public int CdpId { get; set; }
        public int Instancia { get; set; }
        public long Cdp { get; set; }
        public long Crp { get; set; }
        public long Obligacion { get; set; }
        public long OrdenPago { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaFormato { get; set; }
        public string IdentificacionRubro { get; set; }
        public string IdentificacionUsoPresupuestal { get; set; }
        public string NombreRubro { get; set; }
        public decimal ValorInicial { get; set; }
        public decimal Operacion { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoActual { get; set; }
        public string Detalle1 { get; set; }
        public string Detalle2 { get; set; }
        public string Detalle3 { get; set; }
        public string Detalle4 { get; set; }
        public string Detalle5 { get; set; }
        public string Detalle6 { get; set; }
        public string Detalle7 { get; set; }
        public string Detalle8 { get; set; }
        public string Detalle9 { get; set; }
        public string Detalle10 { get; set; }
        public string Pci { get; set; }
        public int CantidadMaxima { get; set; }
        public decimal ValorPagadoFechaActual { get; set; }
        public int NumeroPagoFechaActual { get; set; }
        public int TipoIdentificacionTercero { get; set; }
        public int TerceroId { get; set; }
        public string NumeroIdentificacionTercero { get; set; }
        public string NombreTercero { get; set; }
        public int FormatoSolicitudPagoId { get; set; }
        public string Objeto { get; set; }
        public int? SupervisorId { get; set; }
        public int? ContratoId { get; set; }
        public int PciId { get; set; }
        public int PlanPagoId { get; set; }
        public decimal ValorFacturado { get; set; }
        public string NumeroRadicadoSupervisor { get; set; }
        public DateTime? FechaRadicadoSupervisor { get; set; }
        public long NumeroDocumento { get; set; }

    }
}
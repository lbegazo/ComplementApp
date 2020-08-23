using System;

namespace ComplementApp.API.Dtos
{
    public class PlanPagoDto
    {
        public int PlanPagoId { get; set; }

        public long Cdp { get; set; }

        public long Crp { get; set; }

        public int AnioPago { get; set; }

        public int MesPago { get; set; }

        public decimal ValorInicial { get; set; }

        public decimal ValorAdicion { get; set; }

        public decimal ValorAPagar { get; set; }

        public decimal ValorPagado { get; set; }

        public string Viaticos { get; set; }

        public int NumeroPago { get; set; }

        public string NumeroRadicadoProveedor { get; set; }

        public DateTime FechaRadicadoProveedor { get; set; }

        public string NumeroRadicadoSupervisor { get; set; }

        public DateTime FechaRadicadoSupervisor { get; set; }        

        public string NumeroFactura { get; set; }

        public decimal ValorFacturado { get; set; }

        public string Observaciones { get; set; }

        public DateTime FechaFactura { get; set; }

        public long Obligacion { get; set; }

        public DateTime FechaObligacion { get; set; }

        public long OrdenPago { get; set; }

        public DateTime FechaOrdenPago { get; set; }

        public int DiasAlPago { get; set; }

        public int EstadoId { get; set; }

        public string EstadoPlanPago { get; set; }

        public int TipoIdentificacionTercero { get; set; }

        public string IdentificacionTercero { get; set; }

        public int RubroPresupuestalId { get; set; }

        public string IdentificacionRubroPresupuestal { get; set; }

         public int UsoPresupuestalId { get; set; }

        public string IdentificacionUsoPresupuestal { get; set; }

        public string EstadoOrdenPago { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPlanPago")]
    public class PlanPago
    {
        public int PlanPagoId { get; set; }

        public long Cdp { get; set; }

        public long Crp { get; set; }

        public int AnioPago { get; set; }

        public int MesPago { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorInicial { get; set; }

         [Column(TypeName = "decimal(30,8)")]
        public decimal? ValorAdicion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorAPagar { get; set; }

         [Column(TypeName = "decimal(30,8)")]
        public decimal? ValorPagado { get; set; }

        public bool Viaticos { get; set; }

        public int NumeroPago { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string NumeroRadicadoProveedor { get; set; }

        public DateTime? FechaRadicadoProveedor { get; set; }

         [Column(TypeName = "VARCHAR(250)")]
        public string NumeroRadicadoSupervisor { get; set; }

        public DateTime? FechaRadicadoSupervisor { get; set; }        

        [Column(TypeName = "VARCHAR(250)")]
        public string NumeroFactura { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal? ValorFacturado { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Observaciones { get; set; }

        public DateTime? FechaFactura { get; set; }

        public long? Obligacion { get; set; }

        public long? OrdenPago { get; set; }

        public DateTime? FechaOrdenPago { get; set; }

        public int? DiasAlPago { get; set; }

        public int? EstadoPlanPagoId { get; set; }

        public int? EstadoOrdenPagoId { get; set; }

        public int TerceroId { get; set; }

        public Tercero Tercero { get; set; }

        public int RubroPresupuestalId { get; set; }

        public RubroPresupuestal RubroPresupuestal { get; set; }

         public int UsoPresupuestalId { get; set; }

        public UsoPresupuestal UsoPresupuestal { get; set; }

    }
}
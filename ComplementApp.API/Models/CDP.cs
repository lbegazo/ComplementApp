using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TCDP")]
    public class CDP
    {
        public int CdpId { get; set; }

        public int Instancia { get; set; }

        public long Cdp { get; set; }

        public long Crp { get; set; }

        public long Obligacion { get; set; }

        public long OrdenPago { get; set; }

        public DateTime Fecha { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle1 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorInicial { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal Operacion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorTotal { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoActual { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle2 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle3 { get; set; }

        [Column(TypeName = "VARCHAR(4000)")]
        public string Detalle4 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle5 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle6 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle7 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle8 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle9 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Detalle10 { get; set; }

        public int RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public int TerceroId { get; set; }

        public Tercero Tercero { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ComplementApp.API.Models
{
    [Table("TDetalleSolicitudCDP")]
    public class DetalleSolicitudCDP
    {
        public int DetalleSolicitudCDPId { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoActividad { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal? ValorActividad { get; set; }        

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorSolicitud { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal? ValorCDP { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal? SaldoCDP { get; set; }

        public int RubroPresupuestalId { get; set; }

        public RubroPresupuestal RubroPresupuestal { get; set; }

        public int SolicitudCDPId { get; set; }

        public SolicitudCDP SolicitudCDP { get; set; }
    }
}
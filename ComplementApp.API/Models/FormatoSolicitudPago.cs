using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TFormatoSolicitudPago")]
    public class FormatoSolicitudPago
    {
        public int FormatoSolicitudPagoId { get; set; }

        [Required]
        public int TerceroId { get; set; }

        [Required]
        public int PlanPagoId { get; set; }
        public PlanPago PlanPago { get; set; }

        [Required]
        public long Crp { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFinal { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorFacturado { get; set; }

        [Required]
        public int MesId { get; set; }

        [Required]
        public int ActividadEconomicaId { get; set; }

        public ActividadEconomica ActividadEconomica { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string NumeroPlanilla { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroFactura { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Observaciones { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal BaseCotizacion { get; set; }

        [Required]
        public int EstadoId { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string ObservacionesModificacion { get; set; }
        public int? SupervisorId { get; set; }

        public int? UsuarioIdRegistro { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public int? UsuarioIdModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorBaseGravableRenta { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorIva { get; set; }
        public ICollection<DetalleFormatoSolicitudPago> DetallesFormatoSolicitudPago { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
        public bool EsSaludVencida { get; set; }
    }
}
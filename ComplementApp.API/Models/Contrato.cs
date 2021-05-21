using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TContrato")]
    public class Contrato
    {
        public int ContratoId { get; set; }

        [Required]
        public long Crp { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string NumeroContrato { get; set; }

        public int TipoContratoId { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFinal { get; set; }

        [Required]
        public DateTime FechaExpedicionPoliza { get; set; }

        [Required]
        public int Supervisor1Id { get; set; }
        public int? Supervisor2Id { get; set; }
        public int UsuarioIdRegistro { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public int UsuarioIdModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorPagoMensual { get; set; }
    }
}
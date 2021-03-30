using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TArchivoDetalleLiquidacion")]
    public class ArchivoDetalleLiquidacion
    {
        public int ArchivoDetalleLiquidacionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }

        [Required]
        public int Consecutivo { get; set; }

        [Required]
        public DateTime FechaGeneracion { get; set; }

        [Required]
        public int CantidadRegistro { get; set; }

        [Required]
        public int UsuarioIdRegistro { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public int UsuarioIdModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int TipoDocumentoArchivo { get; set; }

        public ICollection<DetalleArchivoLiquidacion> DetalleArchivo { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
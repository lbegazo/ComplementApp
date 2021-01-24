using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TTercero")]
    public class Tercero
    {
        public int TerceroId { get; set; }

        [Required]
        public int TipoIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string NumeroIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        public DateTime FechaExpedicionDocumento { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string RegimenTributario { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Direccion { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Email { get; set; }

        [Column(TypeName = "VARCHAR(20)")]
        public string Telefono { get; set; }

        public bool DeclaranteRenta { get; set; }

        public bool FacturadorElectronico { get; set; }

        [NotMapped]
        public int ModalidadContrato { get; set; }

        [NotMapped]
        public int TipoPago { get; set; }
        
        [NotMapped]
        public int TipoIva { get; set; }

        public ICollection<TerceroDeduccion> DeduccionesXTercero { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDeduccion")]
    public class Deduccion
    {
        public int DeduccionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Tarifa { get; set; }

        public bool Gmf { get; set; }

        public bool estado { get; set; }

        public int TipoBaseDeduccionId { get; set; }

        public TipoBaseDeduccion TipoBaseDeduccion { get; set; }
        public int? TerceroId { get; set; }
        public Tercero Tercero { get; set; }
        public ICollection<TerceroDeduccion> DeduccionesXTercero { get; set; }
        public bool EsValorFijo { get; set; }        
    }
}
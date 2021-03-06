using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TTipoBaseDeduccion")]
    public class TipoBaseDeduccion
    {
        public int TipoBaseDeduccionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }
    }
}
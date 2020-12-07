using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TFuenteFinanciacion")]
    public class FuenteFinanciacion
    {
        public int FuenteFinanciacionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
    }
}
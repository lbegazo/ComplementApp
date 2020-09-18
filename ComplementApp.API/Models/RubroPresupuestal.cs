using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TRubroPresupuestal")]
    public class RubroPresupuestal
    {
        public int RubroPresupuestalId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Identificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Required]
        public int? PadreRubroId { get; set; }
    }
}
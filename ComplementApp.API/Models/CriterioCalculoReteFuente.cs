using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
     [Table("TCriterioCalculoReteFuente")]
    public class CriterioCalculoReteFuente
    {

        public int CriterioCalculoReteFuenteId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Tarifa { get; set; }

        [Required]
        public int Desde { get; set; }

        [Required]
        public int Hasta { get; set; }

        [Required]
        public int Factor { get; set; }
    }
}
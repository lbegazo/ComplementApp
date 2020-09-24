using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
     [Table("TCriterioCalculoReteFuente")]
    public class CriterioCalculoReteFuente
    {

        public int CriterioCalculoReteFuenteId { get; set; }
        [Required]
        public decimal Tarifa { get; set; }

        [Required]
        public int Desde { get; set; }

        [Required]
        public int Hasta { get; set; }

        [Required]
        public int Factor { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TTerceroDeduccion")]
    public class TerceroDeduccion
    {
        public int TerceroDeduccionId { get; set; }
        public int TerceroId { get; set; }
        public Tercero Tercero { get; set; }
        public int? DeduccionId { get; set; }
        public Deduccion Deduccion { get; set; }
        public int? ActividadEconomicaId { get; set; }
        public ActividadEconomica ActividadEconomica { get; set; }
        public int? TerceroDeDeduccionId { get; set; }
    }
}
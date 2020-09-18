using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TTerceroDeduccion")]
    public class TerceroDeduccion
    {
        public int TerceroId { get; set; }
        public Tercero Tercero { get; set; }

        public int DeduccionId { get; set; }
        public Deduccion Deduccion { get; set; }
    }
}
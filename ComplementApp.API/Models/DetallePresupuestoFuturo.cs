using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetallePresupuestoFuturo")]
    public class DetallePresupuestoFuturo
    {
        public int DetallePresupuestoFuturoId { get; set; }

        public int PresupuestoFuturoId { get; set; }
        public PresupuestoFuturo PresupuestoFuturo { get; set; }
        public int DecretoFuturoId { get; set; }
        public DecretoFuturo DecretoFuturo { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorDecretoFuturo { get; set; }        
    }
}
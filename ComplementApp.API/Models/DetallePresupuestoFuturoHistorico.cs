using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetallePresupuestoFuturoHistorico")]
    public class DetallePresupuestoFuturoHistorico
    {
        [Key]
        public int DetallePresupuestoFuturoId { get; set; }

        public int PresupuestoFuturoId { get; set; }
        public int DecretoFuturoId { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorDecretoFuturo { get; set; }        
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetalleFormatoSolicitudPago")]
    public class DetalleFormatoSolicitudPago
    {
        public int DetalleFormatoSolicitudPagoId { get; set; }
        public int FormatoSolicitudPagoId { get; set; }
        public FormatoSolicitudPago FormatoSolicitudPago { get; set; }
        public int RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorAPagar { get; set; }
   
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetalleArchivoLiquidacion")]
    public class DetalleArchivoLiquidacion
    {
        public int ArchivoDetalleLiquidacionId { get; set; }

        public ArchivoDetalleLiquidacion ArchivoDetalleLiquidacion { get; set; }

        public int DetalleLiquidacionId { get; set; }

        public DetalleLiquidacion DetalleLiquidacion { get; set; }
    }
}
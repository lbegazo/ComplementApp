using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TLiquidacionDeduccion")]
    public class LiquidacionDeduccion
    {
        public int LiquidacionDeduccionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
        public Deduccion Deduccion { get; set; }

        [Required]
        public int DeduccionId { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Tarifa { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Base { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Valor { get; set; }

        public int DetalleLiquidacionId { get; set; }

        public DetalleLiquidacion DetalleLiquidacion { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TIndicador")]
    public class DetalleDecretoFuturo
    {
        public int DetalleDecretoFuturoId { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorApropiacionVigente { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoPorProgramar { get; set; }

        public int? RubroPresupuestalId { get; set; }

        public RubroPresupuestal RubroPresupuestal { get; set; }

        public int DecretoFuturoId { get; set; }

        public DecretoFuturo DecretoFuturo { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
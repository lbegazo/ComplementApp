using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDecretoFuturo")]
    public class DecretoFuturo
    {
         public int DecretoFuturoId { get; set; }        

        [Column(TypeName = "decimal(30,8)")]
        public decimal ApropiacionVigente { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ApropiacionDisponible { get; set; }    
        public int?  RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public ICollection<DetalleDecretoFuturo> DetalleDecretoFuturo { get; set; }  
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
        public int? FuenteFinanciacionId { get; set; }
        public int? SituacionFondoId { get; set; }
        public int? RecursoPresupuestalId { get; set; }
        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoProgramado { get; set; }
        public int?  Anio { get; set; }
    }
}
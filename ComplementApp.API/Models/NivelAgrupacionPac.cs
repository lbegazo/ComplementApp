using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TNivelAgrupacionPac")]
    public class NivelAgrupacionPac
    {
        public int NivelAgrupacionPacId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Identificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
        public int RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public int SituacionFondoId { get; set; }
        public SituacionFondo SituacionFondo { get; set; }
        public int FuenteFinanciacionId { get; set; }
        public FuenteFinanciacion FuenteFinanciacion { get; set; }
        public int RecursoPresupuestalId { get; set; }
        public RecursoPresupuestal RecursoPresupuestal { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string DependenciaAfectacionPAC { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string IdentificacionTesoreria { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TClavePresupuestalContable")]
    public class ClavePresupuestalContable
    {
        public int ClavePresupuestalContableId { get; set; }

        [Required]
        public long Crp { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Pci { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Dependencia { get; set; }

        [Required]
        public int RubroPresupuestalId { get; set; }

        public RubroPresupuestal RubroPresupuestal { get; set; }

        [Required]
        public int TerceroId { get; set; }

        public Tercero Tercero { get; set; }

        [Required]
        public int SituacionFondoId { get; set; }

        public SituacionFondo SituacionFondo { get; set; }

        [Required]
        public int FuenteFinanciacionId { get; set; }

        public FuenteFinanciacion FuenteFinanciacion { get; set; }

        [Required]
        public int RecursoPresupuestalId { get; set; }

        public RecursoPresupuestal RecursoPresupuestal { get; set; }

        [Required]
        public int UsoPresupuestalId { get; set; }

        public UsoPresupuestal UsoPresupuestal { get; set; }

        [Required]
        public int RelacionContableId { get; set; }

        public RelacionContable RelacionContable { get; set; }

    }
}
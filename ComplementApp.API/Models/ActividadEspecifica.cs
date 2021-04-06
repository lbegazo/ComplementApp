using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{

    [Table("TActividadEspecifica")]

    public class ActividadEspecifica
    {
        public int ActividadEspecificaId { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorApropiacionVigente { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoPorProgramar { get; set; }

        public int? RubroPresupuestalId { get; set; }

        public RubroPresupuestal RubroPresupuestal { get; set; }

        public int ActividadGeneralId { get; set; }

        public ActividadGeneral ActividadGeneral { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }

        [NotMapped]
        public int EstadoModificacion { get; set; }
    }
}
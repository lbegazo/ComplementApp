using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TActividadGeneral")]
    public class ActividadGeneral
    {
        public int ActividadGeneralId { get; set; }        

        [Column(TypeName = "decimal(30,8)")]
        public decimal ApropiacionVigente { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ApropiacionDisponible { get; set; }    
        public int?  RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        
        [NotMapped]
        public int EstadoModificacion { get; set; }
    }
}
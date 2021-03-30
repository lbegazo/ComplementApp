using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TUsoPresupuestal")]
    public class UsoPresupuestal
    {
        public int UsoPresupuestalId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Identificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Required]
        public bool MarcaAusteridad { get; set; }
        public int? RubroPresupuestalId { get; set; }    
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDependencia")]
    public class Dependencia
    {   
        public int DependenciaId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }       

        public int AreaId { get; set; }

        public Area Area { get; set; } 
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
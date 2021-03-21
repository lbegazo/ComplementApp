using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPci")]
    public class Pci
    {
        public int PciId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Identificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}
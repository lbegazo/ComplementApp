using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TAtributoContable")]
    public class AtributoContable
    {
         public int AtributoContableId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
    }
}
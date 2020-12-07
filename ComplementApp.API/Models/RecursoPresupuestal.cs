using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TRecursoPresupuestal")]
    public class RecursoPresupuestal
    {
        public int RecursoPresupuestalId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }
    }
}
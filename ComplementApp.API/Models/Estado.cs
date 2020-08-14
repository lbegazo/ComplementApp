using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TEstado")]
    public class Estado
    {
        public int EstadoId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string TipoDocumento { get; set; }
    }
}
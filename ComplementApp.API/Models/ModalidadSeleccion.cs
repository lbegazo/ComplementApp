using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TModalidadSeleccion")]
    public class ModalidadSeleccion
    {
        public int ModalidadSeleccionId{ get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TParametroSistema")]
    public class ParametroSistema
    {
        public int ParametroSistemaId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Descripcion { get; set; }

        [Column(TypeName = "VARCHAR(8000)")]
        public string Valor { get; set; }

    }
}
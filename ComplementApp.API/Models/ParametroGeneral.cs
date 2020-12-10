using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TParametroGeneral")]
    public class ParametroGeneral
    {
        public int ParametroGeneralId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }


        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Descripcion { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string Valor { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Tipo { get; set; }


    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TTipoGasto")]
    public class TipoGasto
    {
        public int TipoGastoId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }
    }
}
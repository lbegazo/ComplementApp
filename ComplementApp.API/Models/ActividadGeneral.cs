using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TActividadGeneral")]
    public class ActividadGeneral
    {
        public int ActividadGeneralId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorApropiacion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoActual { get; set; }
    }
}
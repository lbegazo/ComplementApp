using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TActividadEconomica")]
    public class ActividadEconomica
    {
        public int ActividadEconomicaId { get; set; }

        [Column(TypeName = "VARCHAR(4)")]
        public string Codigo { get; set; }

        [Column(TypeName = "VARCHAR(1000)")]
        public string Nombre { get; set; }

    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_Area")]
    public class Area
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public bool EsAdministrable { get; set; }
    }
}
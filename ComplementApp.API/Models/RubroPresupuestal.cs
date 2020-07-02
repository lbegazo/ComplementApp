using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_RubroPresupuestal")]
    public class RubroPresupuestal
    {
        public int Id { get; set; }

        public string Identificacion { get; set; }

        public string Descripcion { get; set; }
    }
}
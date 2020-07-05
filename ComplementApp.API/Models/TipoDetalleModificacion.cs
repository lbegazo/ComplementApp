using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_TipoDetalleModificacion")]
    public class TipoDetalleModificacion
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

    }
}
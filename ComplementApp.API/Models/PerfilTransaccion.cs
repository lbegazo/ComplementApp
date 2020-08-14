using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPerfilTransaccion")]
    public class PerfilTransaccion
    {
        public int PerfilId { get; set; }

        public Perfil Perfil { get; set; }

        public int TransaccionId { get; set; }

        public Transaccion Transaccion { get; set; }
    }
}
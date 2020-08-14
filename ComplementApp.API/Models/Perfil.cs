using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPerfil")]
    public class Perfil
    
    {
        public int PerfilId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Codigo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombre { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public ICollection<UsuarioPerfil> UsuarioPerfiles { get; set; }

        public ICollection<PerfilTransaccion> PerfilTransacciones { get; set; }
    }
}
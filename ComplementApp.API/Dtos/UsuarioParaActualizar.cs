using System;
using System.ComponentModel.DataAnnotations;

namespace ComplementApp.API.Dtos
{
    public class UsuarioParaActualizar
    {
        
        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public int CargoId { get; set; }

        [Required]
        public int AreaId { get; set; }

    }
}
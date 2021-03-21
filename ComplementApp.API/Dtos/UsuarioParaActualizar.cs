using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ComplementApp.API.Models;

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
        
        [Required]
        public int PciId { get; set; }

        [Required]
        public ICollection<Perfil> Perfiles { get; set; }

    }
}
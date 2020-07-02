using System;
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class UsuarioParaDetalleDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

         public DateTime FechaCreacion { get; set; }

        public DateTime FechaUltimoAcceso { get; set; }
        
        public string Nombres { get; set; }
    
        public string Apellidos { get; set; }
   
        public int CargoId { get; set; }

        public int AreaId { get; set; }
    }
}
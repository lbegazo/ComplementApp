using System;
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class UsuarioParaDetalleDto
    {
        private string _nombreCompleto;
        public int UsuarioId { get; set; }

        public string Username { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaUltimoAcceso { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public int CargoId { get; set; }

        public int AreaId { get; set; }

        public string CargoNombre { get; set; }

        public string AreaNombre { get; set; }

        public ICollection<Perfil> Perfiles { get; set; }

        public string NombreCompleto
        {
            get { return _nombreCompleto; }
            set
            {
                _nombreCompleto = Nombres + " " + Apellidos;
            }
        }
    }
}
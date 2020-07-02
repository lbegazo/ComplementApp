using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_Usuario")]
    public class Usuario
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateTime FechaUltimoAcceso { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Nombres { get; set; }

        public string Apellidos { get; set; }

        public int CargoId { get; set; }

        public Cargo Cargo { get; set; }

        public int AreaId { get; set; }

        public Area Area { get; set; }

    }
}
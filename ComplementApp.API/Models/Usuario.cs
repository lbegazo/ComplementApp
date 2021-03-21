using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TUsuario")]
    public class Usuario
    {
        public int UsuarioId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string Username { get; set; }

        [NotMapped]
        public string  Password { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
        public DateTime FechaUltimoAcceso { get; set; }
        public DateTime FechaCreacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Nombres { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Apellidos { get; set; }
        public int? CargoId { get; set; }
        public Cargo Cargo { get; set; }
        public int? AreaId { get; set; }
        public Area Area { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Email {get; set;}
        public int? TerceroId { get; set; }
        public Tercero Tercero { get; set; }
        public ICollection<UsuarioPerfil> UsuarioPerfiles { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
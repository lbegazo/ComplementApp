using System;
using System.ComponentModel.DataAnnotations;

namespace ComplementApp.API.Dtos
{
    public class UsuarioParaRegistrarDto
    {
        public UsuarioParaRegistrarDto()
        {
            FechaCreacion = DateTime.Now;
            //FechaUltimoAcceso = DateTime.Now;
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(  8, 
                        MinimumLength = 4, 
                        ErrorMessage = "Se debe especificar una contraseña entre 4 y 8 caracteres")]
        public string Password { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaUltimoAcceso { get; set; }

        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public int CargoId { get; set; }

        [Required]
        public int AreaId { get; set; }

        [Required]
        public bool EsAdministrador { get; set; }



    }
}
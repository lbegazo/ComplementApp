using System.ComponentModel.DataAnnotations;

namespace ComplementApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string  UserName { get; set; }

        [Required]
        [StringLength(8,MinimumLength=4, ErrorMessage="Se debe especificar una contraseña entre 4 y 8 caracteres")]
        public string Password { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TCuentaContable")]
    public class CuentaContable
    {
        public int CuentaContableId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string NumeroCuenta { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string DescripcionCuenta { get; set; }
    }
}
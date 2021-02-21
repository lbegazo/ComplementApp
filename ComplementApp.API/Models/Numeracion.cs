using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TNumeracion")]
    public class Numeracion
    {
        public int NumeracionId { get; set; }
        public int Consecutivo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string NumeroConsecutivo { get; set; }
        public int? FormatoSolicitudPagoId { get; set; }
        public FormatoSolicitudPago FormatoSolicitudPago { get; set; }
        public bool Utilizado { get; set; }
    }
}
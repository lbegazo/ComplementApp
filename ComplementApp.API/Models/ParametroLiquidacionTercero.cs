using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TParametroLiquidacionTercero")]
    public class ParametroLiquidacionTercero
    {
        public int ParametroLiquidacionTerceroId { get; set; }

        [Required]
        public int ModalidadContrato { get; set; }

        [Required]
        public int TipoPago { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal? HonorarioSinIva { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal BaseAporteSalud { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal AporteSalud { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal AportePension { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal RiesgoLaboral { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal FondoSolidaridad { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal PensionVoluntaria { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Dependiente { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal Afc { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal MedicinaPrepagada { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal InteresVivienda { get; set; }

        public DateTime? FechaInicioDescuentoInteresVivienda { get; set; }

        public DateTime? FechaFinalDescuentoInteresVivienda { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal TarifaIva { get; set; }

        public int? TipoIva { get; set; }

        public int? TipoCuentaPorPagar { get; set; }

        [Required]
        public int TipoDocumentoSoporte { get; set; }

        public string Debito { get; set; }

        public string Credito { get; set; }

        public string NumeroCuenta { get; set; }

        public string TipoCuenta { get; set; }

        public int? ConvenioFontic { get; set; }

        public int TerceroId { get; set; }

        public Tercero Tercero { get; set; }

        public bool GmfAfc { get; set; }
    }
}
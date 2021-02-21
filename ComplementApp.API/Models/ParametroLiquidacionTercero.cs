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

        public int TerceroId { get; set; }
        public Tercero Tercero { get; set; }
        public bool GmfAfc { get; set; }
        public int UsuarioIdRegistro { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int UsuarioIdModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool FacturaElectronica { get; set; }
        public bool Subcontrata { get; set; }
        public int? SupervisorId { get; set; }
        public Usuario Supervisor { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal OtrosDescuentos { get; set; }
        public DateTime? FechaInicioOtrosDescuentos { get; set; }
        public DateTime? FechaFinalOtrosDescuentos { get; set; }
        public bool NotaLegal1 { get; set; }
        public bool NotaLegal2 { get; set; }
        public bool NotaLegal3 { get; set; }
        public bool NotaLegal4 { get; set; }
        public bool NotaLegal5 { get; set; }
        public bool NotaLegal6 { get; set; }
        public int? TipoAdminPilaId { get; set; }
        public TipoAdminPila TipoAdminPila { get; set; }
        
    }
}
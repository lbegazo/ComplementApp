using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TCargaObligacion")]
    public class CargaObligacion
    {
        public int CargaObligacionId { get; set; }

        [Required]
        public int NumeroDocumento { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string Estado { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorActual { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorDeduccion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorObligadoNoOrdenado { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string NumeroIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string NombreRazonSocial { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string MedioPago { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string TipoCuenta { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string NumeroCuenta { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string EstadoCuenta { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string EntidadNit { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string EntidadDescripcion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string Dependencia { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string DependenciaDescripcion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorInicial { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorOperacion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorActual2 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoPorUtilizar { get; set; }        

        [Required]
        [Column(TypeName = "VARCHAR(1000)")]
        public string Concepto { get; set; }

        [Required]
        public int SolicitudCdp { get; set; }

        [Required]
        public int Cdp { get; set; }

        [Required]
        public int Compromiso { get; set; }
        public int? CuentaPorPagar { get; set; }
        public DateTime? FechaCuentaPorPagar { get; set; }

        [Required]
        public int Obligacion { get; set; }

        [Required]
        public long OrdenPago { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Reintegro { get; set; }
        public DateTime FechaDocSoporteCompromiso { get; set; }
        
        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroDocSoporteCompromiso { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string ObjetoCompromiso { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
        public int? RubroPresupuestalId { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public int? FuenteFinanciacionId { get; set; }
        public FuenteFinanciacion FuenteFinanciacion { get; set; }
        
        public int? SituacionFondoId { get; set; }
        public SituacionFondo SituacionFondo { get; set; }

        public int? RecursoPresupuestalId { get; set; }
        public RecursoPresupuestal RecursoPresupuestal { get; set; }
        public int? TipoDocumentoIdentidadId { get; set; }
        public TipoDocumentoIdentidad TipoDocumentoIdentidad { get; set; }
        public int? TipoDocumentoSoporteId { get; set; }
        public TipoDocumentoSoporte TipoDocumentoSoporte { get; set; }
    }
}
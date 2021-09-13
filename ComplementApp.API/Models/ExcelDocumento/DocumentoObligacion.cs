using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models.ExcelDocumento
{
    [Table("TDocumentoObligacion")]
    public class DocumentoObligacion
    {
        public int DocumentoObligacionId { get; set; }

        [Required]
        public long NumeroDocumento { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Estado { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorActual { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorDeduccion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorObligadoNoOrdenado { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string NumeroIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string NombreRazonSocial { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string MedioPago { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoCuenta { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroCuenta { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string EstadoCuenta { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string EntidadNit { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string EntidadDescripcion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string Dependencia { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string DependenciaDescripcion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string IdentificacionRubroPresupuestal { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string DescripcionRubroPresupuestal { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorInicial { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorOperacion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorActual2 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoPorUtilizar { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string FuenteFinanciacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string SituacionFondo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string RecursoPresupuestal { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(3000)")]
        public string Concepto { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string SolicitudCdp { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Cdp { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Compromisos { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string CuentasXPagar { get; set; }

        public DateTime FechaCuentaXPagar { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Obligaciones { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string OrdenesPago { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Reintegros { get; set; }

        public DateTime FechaDocumentoSoporte { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoDocumentoSoporte { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroDocumentoSoporte { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string ObjetoCompromiso { get; set; }
    }
}
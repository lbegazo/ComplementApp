using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models.ExcelDocumento
{
    [Table("TDocumentoOrdenPago")]
    public class DocumentoOrdenPago
    {
        public int DocumentoOrdenPagoId { get; set; }

        [Required]
        public long NumeroDocumento { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Estado { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorBruto { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorDeduccion { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorNeto { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoBeneficiario { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string VigenciaPresupuestal { get; set; }

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

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string FuenteFinanciacion { get; set; }        

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string RecursoPresupuestal { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string SituacionFondo { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorPesos { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorMoneda { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorReintegradoPesos { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorReintegradoMoneda { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string TesoreriaPagadora { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string IdentificacionPagaduria { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string CuentaPagaduria { get; set; }

        [Column(TypeName = "VARCHAR(10)")]
        public string Endosada { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoIdentificacion1 { get; set; }

        [Column(TypeName = "VARCHAR(20)")]
        public string NumeroIdentificacion1 { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string NombreRazonSocial1 { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroCuenta1 { get; set; }

        [Column(TypeName = "VARCHAR(1000)")]
        public string ConceptoPago { get; set; }

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

        public DateTime FechaDocumentoSoporteCompromiso { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string TipoDocumentoSoporteCompromiso { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string NumeroDocumentoSoporteCompromiso { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string ObjetoCompromiso { get; set; }
    }
}
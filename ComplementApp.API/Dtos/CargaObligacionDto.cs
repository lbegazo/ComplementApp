using System;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class CargaObligacionDto
    {
        public int CargaObligacionId { get; set; }
        public int NumeroDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaLimitePago { get; set; }
        public string Estado { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorDeduccion { get; set; }
        public decimal ValorObligadoNoOrdenado { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public TerceroDto Tercero { get; set; }
        public string MedioPago { get; set; }
        public string TipoCuenta { get; set; }
        public string NumeroCuenta { get; set; }
        public string EstadoCuenta { get; set; }
        public string EntidadNit { get; set; }
        public string EntidadDescripcion { get; set; }
        public string Dependencia { get; set; }
        public string DependenciaDescripcion { get; set; }
        public RubroPresupuestalDto RubroPresupuestal { get; set; }
        public decimal ValorInicial { get; set; }
        public decimal ValorOperacion { get; set; }

        public decimal ValorActual2 { get; set; }
        public decimal SaldoPorUtilizar { get; set; }
        public FuenteFinanciacion FuenteFinanciacion { get; set; }
        public SituacionFondo SituacionFondo { get; set; }

        public RecursoPresupuestal RecursoPresupuestal { get; set; }

        public string Concepto { get; set; }

        public int SolicitudCdp { get; set; }

        public int Cdp { get; set; }
        public int Compromiso { get; set; }
        public int? CuentaPorPagar { get; set; }
        public DateTime? FechaCuentaPorPagar { get; set; }

        public int Obligacion { get; set; }
        public long OrdenPago { get; set; }

        public string Reintegro { get; set; }

        public DateTime FechaDocSoporteCompromiso { get; set; }
        public string TipoDocSoporteCompromiso { get; set; }

        public string NumeroDocSoporteCompromiso { get; set; }
        public string ObjetoCompromiso { get; set; }
        public string NombreFuncionario { get; set; }
        public string CargoFuncionario { get; set; }
        public string CodigoPosicionPac { get; set; }
        public string CodigoDependenciaAfectacionPac { get; set; }
        public string CodigoPciTesoreria { get; set; }
        public string CodigoTipoBeneficiario { get; set; }        
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
    }
}
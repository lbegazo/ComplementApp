using System;
using System.Collections.Generic;

namespace ComplementApp.API.Dtos
{
    public class ParametroLiquidacionTerceroDto
    {
        public int ParametroLiquidacionTerceroId { get; set; }
        public int TerceroId { get; set; }
        public int TipoDocumentoIdentidadId { get; set; }
        public string TipoDocumentoIdentidad { get; set; }
        public string IdentificacionTercero { get; set; }
        public string NombreTercero { get; set; }
        public int ModalidadContrato { get; set; }
        public string ModalidadContratoDescripcion { get; set; }
        public int TipoPago { get; set; }
        public decimal? HonorarioSinIva { get; set; }
        public decimal BaseAporteSalud { get; set; }
        public decimal AporteSalud { get; set; }
        public decimal AportePension { get; set; }
        public decimal RiesgoLaboral { get; set; }
        public decimal FondoSolidaridad { get; set; }
        public decimal PensionVoluntaria { get; set; }
        public decimal Dependiente { get; set; }
        public decimal Afc { get; set; }
        public decimal MedicinaPrepagada { get; set; }
        public decimal InteresVivienda { get; set; }
        public string FechaInicioDescuentoInteresViviendaDes { get; set; }
        public string FechaFinalDescuentoInteresViviendaDes { get; set; }
        public DateTime? FechaInicioDescuentoInteresVivienda { get; set; }
        public DateTime? FechaFinalDescuentoInteresVivienda { get; set; }
        public decimal OtrosDescuentos { get; set; }
        public DateTime? FechaInicioOtrosDescuentos { get; set; }
        public DateTime? FechaFinalOtrosDescuentos { get; set; }
        public decimal TarifaIva { get; set; }
        public int? TipoIva { get; set; }
        public int TipoCuentaXPagarId { get; set; }
        public string TipoCuentaPorPagarCodigo { get; set; }
        public string TipoCuentaPorPagarDescripcion { get; set; }
        public int TipoDocumentoSoporteId { get; set; }
        public string TipoDocumentoSoporteDescripcion { get; set; }
        public string Debito { get; set; }
        public string Credito { get; set; }
        public string NumeroCuenta { get; set; }
        public int? ConvenioFontic { get; set; }
        public int? FacturaElectronicaId { get; set; }
        public string FacturaElectronicaDescripcion { get; set; }
        public int? SubcontrataId { get; set; }
        public int? TipoAdminPilaId { get; set; }
        public bool NotaLegal1 { get; set; }
        public bool NotaLegal2 { get; set; }
        public bool NotaLegal3 { get; set; }
        public bool NotaLegal4 { get; set; }
        public bool NotaLegal5 { get; set; }
        public bool NotaLegal6 { get; set; }
        public ICollection<TerceroDeduccionDto> TerceroDeducciones { get; set; }
    }
}
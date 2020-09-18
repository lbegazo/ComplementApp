using System;

namespace ComplementApp.API.Dtos
{
    public class ParametroLiquidacionTerceroDto
    {
        public int TipoIdentificacion { get; set; }
        public string IdentificacionTercero { get; set; }
        public int ModalidadContrato { get; set; }
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

        public string FechaInicioDescuentoInteresVivienda { get; set; }
        public string FechaFinalDescuentoInteresVivienda { get; set; }

        public decimal TarifaIva { get; set; }
        public int? TipoIva { get; set; }
        public int? TipoCuentaPorPagar { get; set; }
        public int TipoDocumentoSoporte { get; set; }
        public string Debito { get; set; }
        public string Credito { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public int? ConvenioFontic { get; set; }

        
        










    }
}
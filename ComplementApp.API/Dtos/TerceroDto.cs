using System;

namespace ComplementApp.API.Dtos
{
    public class TerceroDto
    {
        public int TerceroId { get; set; }

        public int TipoDocumentoIdentidadId { get; set; }

        public string TipoDocumentoIdentidad { get; set; }

        public string NumeroIdentificacion { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public bool DeclaranteRenta { get; set; }

        public string DeclaranteRentaDescripcion { get; set; }

        public bool FacturadorElectronico{ get; set; }  

        public string FacturadorElectronicoDescripcion { get; set; }      

        public string RegimenTributario { get; set; }

        public DateTime? FechaExpedicionDocumento { get; set; }  

        public int ModalidadContrato { get; set; }
        
    }
}
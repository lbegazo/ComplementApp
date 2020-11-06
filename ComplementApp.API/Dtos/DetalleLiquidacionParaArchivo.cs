using System;

namespace ComplementApp.API.Dtos
{
    public class DetalleLiquidacionParaArchivo
    {
        public string PCI { get; set; }

        public string Fecha { get; set; }

        public int TipoIdentificacion { get; set; }

        public string NumeroIdentificacion { get; set; }

        public long Crp { get; set; }

        public int TipoCuentaPagar { get; set; }

        public decimal TotalACancelar { get; set; }

        public decimal ValorIva { get; set; }

        public string TextoComprobanteContable { get; set; }

        public int TipoDocumentoSoporte { get; set; }

         public string NumeroFactura { get; set; }

         public string ConstanteNumero { get; set; }

         public string NombreSupervisor { get; set; }

         public string ConstanteCargo { get; set; }

         public DateTime FechaRegistro { get; set; }
    }
}
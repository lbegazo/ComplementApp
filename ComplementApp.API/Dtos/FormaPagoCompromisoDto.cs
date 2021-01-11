using System.Collections.Generic;

namespace ComplementApp.API.Dtos
{
    public class FormaPagoCompromisoDto
    {
        public CDPDto Cdp { get; set; }
        public ICollection<LineaPlanPagoDto> ListaLineaPlanPago { get; set; }

    }
}
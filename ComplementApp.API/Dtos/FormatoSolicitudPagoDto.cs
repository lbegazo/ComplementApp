using System;
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class FormatoSolicitudPagoDto
    {
        public int FormatoSolicitudPagoId { get; set; }
        public CDPDto Cdp { get; set; }
        public TerceroDto Tercero { get; set; }
        public Contrato Contrato { get; set; }
        public decimal ValorPagadoFechaActual { get; set; }
        public int NumeroPagoFechaActual { get; set; }
        public int CantidadMaxima { get; set; }
        public ICollection<CDPDto> PagosRealizados { get; set; }
    }
}
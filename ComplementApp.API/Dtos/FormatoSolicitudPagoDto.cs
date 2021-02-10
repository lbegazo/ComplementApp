using System;
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class FormatoSolicitudPagoDto
    {
        public int FormatoSolicitudPagoId { get; set; }
        public int PlanPagoId { get; set; }
        public DateTime FechaSistema { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal ValorFacturado { get; set; }
        public int MesId { get; set; }
        public int EstadoId { get; set; }
        public string MesDescripcion { get; set; }
        public string NumeroPlanilla { get; set; }
        public string NumeroFactura { get; set; }
        public string Observaciones { get; set; }
        public decimal ValorBaseGravableRenta { get; set; }
        public decimal ValorIva { get; set; }
        public decimal BaseCotizacion { get; set; }
        public decimal ValorPagadoFechaActual { get; set; }
        public string ObservacionesModificacion { get; set; }
        public decimal AporteSalud { get; set; }
        public decimal AportePension { get; set; }
        public decimal RiesgoLaboral { get; set; }
        public decimal FondoSolidaridad { get; set; }
        public int NumeroPagoFechaActual { get; set; }
        public int CantidadMaxima { get; set; }
        public string NumeroRadicadoProveedor { get; set; }
        public DateTime FechaRadicadoProveedor { get; set; }
        public string NumeroRadicadoSupervisor { get; set; }
        public string ModalidadContrato { get; set; }
        public string TipoAdminPila { get; set; }
        public DateTime FechaRadicadoSupervisor { get; set; }        
        public CDPDto Cdp { get; set; }
        public TerceroDto Tercero { get; set; }
        public ContratoDto Contrato { get; set; }
        public PlanPagoDto PlanPago { get; set; }
        public ValorSeleccion ActividadEconomica { get; set; }
        public ICollection<CDPDto> PagosRealizados { get; set; }
        public ICollection<DetalleFormatoSolicitudPagoDto> detallesFormatoSolicitudPago { get; set; }

    }
}
using System;
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class SolicitudCDPDto
    {
        public int SolicitudCDPId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string EstadoCDP { get; set; }
        public long? Cdp { get; set; }
        public int NumeroActividad { get; set; }
        public bool AplicaContrato { get; set; }
        public string AplicaContratoDescripcion { get; set; }
        public string NombreBienServicio { get; set; }
        public string ProyectoInversion { get; set; }
        public string ActividadProyectoInversion { get; set; }
        public string ObjetoBienServicioContratado { get; set; }
        public string Observaciones { get; set; }
        public TipoDetalleCDP TipoDetalleCDP { get; set; }
        public int? TipoDetalleCDPId { get; set; }
        public TipoOperacion TipoOperacion { get; set; }
        public int TipoOperacionId { get; set; }
        public ICollection<DetalleSolicitudCDP> DetalleSolicitudCDPs { get; set; }
        public UsuarioParaDetalleDto Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
using System;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class SolicitudCDPParaPrincipalDto
    {
        public int SolicitudCDPId { get; set; }
        public string ObjetoBienServicioContratado { get; set; }
        public TipoOperacion TipoOperacion { get; set; }
        public int EstadoSolicitudCDPId { get; set; }
        public UsuarioParaDetalleDto Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ListaValorDTO EstadoSolicitudCDP { get; set; }
    }
}
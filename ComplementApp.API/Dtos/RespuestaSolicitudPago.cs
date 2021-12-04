namespace ComplementApp.API.Dtos
{
    public class RespuestaSolicitudPago
    {
        public int FormatoSolicitudPagoId { get; set; }
        public string NumeroFactura { get; set; }
        public bool Respuesta { get; set; }
        public string Mensaje { get; set; }
    }
}
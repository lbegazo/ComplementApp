using System;

namespace ComplementApp.API.Dtos
{
    public class ContratoDto
    {
        public int ContratoId { get; set; }
        public long Crp { get; set; }
        public string NumeroContrato { get; set; }
        public int TipoContratoId { get; set; }
        public ValorSeleccion TipoContrato { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public DateTime FechaExpedicionPoliza { get; set; }
        public int Supervisor1Id { get; set; }
        public int? Supervisor2Id { get; set; }
        public UsuarioParaDetalleDto Supervisor1 { get; set; }
        public UsuarioParaDetalleDto Supervisor2 { get; set; }
        public bool EsPagoMensual { get; set; }
        public decimal ValorPagoMensual { get; set; }
    }
}
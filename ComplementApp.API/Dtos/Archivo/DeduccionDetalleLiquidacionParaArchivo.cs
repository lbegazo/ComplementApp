namespace ComplementApp.API.Dtos.Archivo
{
    public class DeduccionDetalleLiquidacionParaArchivo
    {
        public int DetalleLiquidacionId { get; set; }
        public int TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string DeduccionCodigo { get; set; }
        public decimal Base { get; set; }
        public decimal Tarifa { get; set; }
        public decimal Valor { get; set; }

    }
}
namespace ComplementApp.API.Dtos
{
    public class TerceroDeduccionDto
    {
        public int TipoIdentificacion { get; set; }
        public string IdentificacionTercero { get; set; }
        public string Codigo { get; set; }

        public int TerceroDeduccionId { get; set; }
        public ValorSeleccion Tercero { get; set; }

        public ValorSeleccion Deduccion { get; set; }

        public ValorSeleccion ActividadEconomica { get; set; }
    }
}
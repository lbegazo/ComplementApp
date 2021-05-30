namespace ComplementApp.API.Dtos
{
    public class TerceroDeduccionDto
    {
        public int TerceroDeduccionId { get; set; }
        public int TipoIdentificacion { get; set; }
        public string IdentificacionTercero { get; set; }
        public string Codigo { get; set; }        
        public ValorSeleccion Tercero { get; set; }
        public DeduccionDto Deduccion { get; set; }
        public ValorSeleccion ActividadEconomica { get; set; }
        public ValorSeleccion TerceroDeDeduccion { get; set; }
        public int EstadoModificacion { get; set; }
        public decimal ValorFijo { get; set; }
    }
}
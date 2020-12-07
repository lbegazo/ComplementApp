namespace ComplementApp.API.Dtos
{
    public class RelacionContableDto
    {
        public int RelacionContableId { get; set; }
        public int? TipoOperacion { get; set; }
        public int? UsoContable { get; set; }
        public ValorSeleccion CuentaContable { get; set; }
        public ValorSeleccion AtributoContable { get; set; }
        public ValorSeleccion TipoGasto { get; set; }
    }
}
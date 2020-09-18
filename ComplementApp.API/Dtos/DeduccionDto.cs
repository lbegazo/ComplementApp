namespace ComplementApp.API.Dtos
{
    public class DeduccionDto
    {
        public int DeduccionId { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public decimal Tarifa { get; set; }

        public string Gmf { get; set; }

        public string Estado { get; set; }

        public string TipoBase { get; set; }

        public decimal Base { get; set; }

        public decimal Valor { get; set; }

    }
}
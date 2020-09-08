namespace ComplementApp.API.Dtos
{
    public class TransaccionDto
    {
        public int TransaccionId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string Ruta { get; set; }
        public string PadreMenu { get; set; }        
    }
}
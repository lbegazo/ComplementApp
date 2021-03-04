namespace ComplementApp.API.Dtos
{
    public class LineaPlanPagoDto
    {
        public int PlanPagoId { get; set; }
        public int MesId { get; set; }
        public string MesDescripcion { get; set; }
        public decimal Valor { get; set; }
        public int EstadoModificacion { get; set; }
        public bool Viaticos { get; set; }
    }
}
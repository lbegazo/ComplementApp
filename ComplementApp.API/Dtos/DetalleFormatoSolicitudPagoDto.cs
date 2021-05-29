namespace ComplementApp.API.Dtos
{
    public class DetalleFormatoSolicitudPagoDto
    {
        public int DetalleFormatoSolicitudPagoId { get; set; }
        public int FormatoSolicitudPagoId { get; set; }
        public ValorSeleccion RubroPresupuestal { get; set; }
        public decimal ValorAPagar { get; set; }
        public string Dependencia { get; set; }
        public int ClavePresupuestalContableId { get; set; }
    }
}
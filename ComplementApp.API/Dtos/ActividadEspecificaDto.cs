namespace ComplementApp.API.Dtos
{
    public class ActividadEspecificaDto
    {
         public int ActividadEspecificaId { get; set; }

        public string Nombre { get; set; }

        public decimal ValorApropiacionVigente { get; set; }

        public decimal SaldoPorProgramar { get; set; }
        
        public string RubroPresupuestal { get; set; }

        public string ActividadGeneral { get; set; }
    }
}
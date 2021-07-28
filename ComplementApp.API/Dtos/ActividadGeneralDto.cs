using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class ActividadGeneralDto
    {
        public int ActividadGeneralId { get; set; }        
        public decimal ApropiacionVigente { get; set; }
        public decimal ApropiacionDisponible { get; set; }    
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public Pci Pci { get; set; }
        public ValorSeleccion FuenteFinanciacion { get; set; }
        public ValorSeleccion SituacionFondo { get; set; }
        public ValorSeleccion RecursoPresupuestal { get; set; }
    }
}
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class ClavePresupuestalContableDto
    {
        public int ClavePresupuestalContableId { get; set; }
        public long Crp { get; set; }
        public string Pci { get; set; }
        public string Dependencia { get; set; }
        public ValorSeleccion RubroPresupuestal { get; set; }
        public ValorSeleccion Tercero { get; set; }
        public ValorSeleccion SituacionFondo { get; set; }
        public ValorSeleccion FuenteFinanciacion { get; set; }
        public ValorSeleccion RecursoPresupuestal { get; set; }
        public ValorSeleccion UsoPresupuestal { get; set; }
        public ValorSeleccion RelacionContable { get; set; }

    }
}
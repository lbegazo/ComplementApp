namespace ComplementApp.API.Dtos.Archivo
{
    public class ClavePresupuestalContableParaArchivo
    {
        public int DetalleLiquidacionId { get; set; }
        public string Dependencia { get; set; }
        public string RubroPresupuestalIdentificacion { get; set; }
        public string RecursoPresupuestalCodigo { get; set; }
        public string SituacionFondoCodigo { get; set; }
        public string FuenteFinanciacionCodigo { get; set; }
        public decimal ValorTotal { get; set; }
        public string AtributoContableCodigo { get; set; }
        public string TipoGastoCodigo { get; set; }
        public string TipoOperacion { get; set; }
        public string UsoContable { get; set; }
        public string NumeroCuenta { get; set; }
    }
}
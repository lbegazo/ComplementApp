namespace ComplementApp.API.Dtos
{
    
    public class DetalleCDPDto
    {
        public int Id { get; set; }
        
        public int Crp { get; set; }

        public int IdArchivo { get; set; }

        public int Cdp { get; set; }

        public int Proy { get; set; }

        public int Prod { get; set; }

        public string Proyecto { get; set; }

        public string ActividadBpin { get; set; }

        public string PlanDeCompras { get; set; }

        public string Responsable { get; set; }

        public string Dependencia { get; set; }

        public string Rubro { get; set; }

        public string RubroDescripcion { get; set; }

        public decimal ValorAct { get; set; }

        public decimal SaldoAct { get; set; }

        public decimal ValorCDP { get; set; }

        public decimal SaldoCDP { get; set; }

        public decimal ValorRP { get; set; }

        public decimal ValorOB { get; set; }

        public decimal ValorOP { get; set; }

        public string Contrato { get; set; }

        public decimal SaldoTotal { get; set; }

        public decimal SaldoDisponible { get; set; }

        public string Area { get; set; }

        public int Paa { get; set; }

        public int IdSofi { get; set; }  
    }
}
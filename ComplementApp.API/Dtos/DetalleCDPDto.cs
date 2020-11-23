namespace ComplementApp.API.Dtos
{
    public class DetalleCDPDto
    {
        public int Id { get; set; }

        public string PcpId { get; set; }

        public int IdArchivo { get; set; }

        public long Cdp { get; set; }

        public int Proy { get; set; }

        public int Prod { get; set; }

        public string Proyecto { get; set; }

        public string ActividadBpin { get; set; }


        public string PlanDeCompras { get; set; }


        public string Responsable { get; set; }


        public string Dependencia { get; set; }

        public int RubroPresupuestalId { get; set; }

        public string IdentificacionRubro { get; set; }

        public string RubroNombre { get; set; }

        public decimal ValorAct { get; set; }


        public decimal SaldoAct { get; set; }

        //Cabecera
        public decimal ValorCDP { get; set; }

        //Cabecera
        public decimal SaldoCDP { get; set; }


        public decimal ValorRP { get; set; }


        public decimal ValorOB { get; set; }


        public decimal ValorOP { get; set; }


        public string AplicaContrato { get; set; }


        public decimal SaldoTotal { get; set; }


        public decimal SaldoDisponible { get; set; }        

        public string Area { get; set; }

        public int Rp { get; set; }

        public decimal Valor_Convenio { get; set; }

        public int Convenio { get; set; }

        public string Decreto { get; set; }
    }
}
using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class DetalleCDPDto
    {
        public int DetalleCdpId { get; set; }
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
        public decimal ValorAct { get; set; }
        public decimal SaldoAct { get; set; }
        public decimal ValorInicial { get; set; }
        //Cabecera
        public decimal ValorCDP { get; set; }
        public decimal ValorModificacion { get; set; }

        //Cabecera
        public decimal SaldoCDP { get; set; }
        public decimal ValorRP { get; set; }
        public decimal ValorOB { get; set; }
        public decimal ValorOP { get; set; }
        public string AplicaContratoDescripcion { get; set; }        
        public bool AplicaContrato { get; set; }
        public decimal SaldoTotal { get; set; }
        public decimal SaldoDisponible { get; set; }
        public string Area { get; set; }
        public long Crp { get; set; }
        public decimal Valor_Convenio { get; set; }
        public int Convenio { get; set; }
        public string Decreto { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public CDP CdpDocumento { get; set; }
        public string DependenciaDescripcion { get; set; }
        public decimal ValorTotal { get; set; }
        public int? ClavePresupuestalContableId { get; set; }
        public string IdentificacionPci { get; set; }
    }
}
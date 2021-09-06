using System;
using ComplementApp.API.Models;

namespace ComplementApp.API.Dtos
{
    public class PlanAdquisicionDto
    {
        public int PlanAdquisicionId { get; set; }

        public string PcpId { get; set; }

        public int IdArchivo { get; set; }

        public long Cdp { get; set; }

        public int Proy { get; set; }

        public int Prod { get; set; }

        public string PlanDeCompras { get; set; }

        public decimal ValorAct { get; set; }

        public decimal SaldoAct { get; set; }

        public decimal ValorCDP { get; set; }

        public decimal ValorRP { get; set; }

        public decimal ValorOB { get; set; }

        public decimal ValorOP { get; set; }

        public bool AplicaContrato { get; set; }

        public decimal SaldoTotal { get; set; }

        public decimal SaldoDisponible { get; set; }
        public long Crp { get; set; }

        public decimal Valor_Convenio { get; set; }
        public int Convenio { get; set; }
        public int ActividadGeneralId { get; set; }
        public int ActividadEspecificaId { get; set; }
        public int UsuarioId { get; set; }

        public int DependenciaId { get; set; }
        public int AreaId { get; set; }
        public int RubroPresupuestalId { get; set; }
        public int DecretoId { get; set; }
        public int PciId { get; set; }
        public int EstadoId { get; set; }
        public int EstadoModificacion { get; set; }
        public ActividadEspecifica ActividadEspecifica { get; set; }
        public ActividadGeneral ActividadGeneral { get; set; }
        public RubroPresupuestal RubroPresupuestal { get; set; }
        public CDP CdpDocumento { get; set; }
        public ValorSeleccion Responsable { get; set; }
        public Pci Pci { get; set; }
        public DateTime FechaEstimadaContratacion { get; set; }

    }
}
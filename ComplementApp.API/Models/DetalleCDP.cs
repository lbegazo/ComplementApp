using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetalleCDP")]
    public class DetalleCDP
    {
        public int DetalleCdpId { get; set; }

        [Column(TypeName = "VARCHAR(10)")]
        public string PcpId { get; set; }

        public int IdArchivo { get; set; }

        public long Cdp { get; set; }

        public int Proy { get; set; }

        public int Prod { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string PlanDeCompras { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorAct { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoAct { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorCDP { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorRP { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorOB { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorOP { get; set; }

        [Column(TypeName = "VARCHAR(10)")]
        public string AplicaContrato { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoTotal { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoDisponible { get; set; }

        public int Rp { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal Valor_Convenio { get; set; }

        public int Convenio { get; set; }

        public int ActividadGeneralId { get; set; }

        public int ActividadEspecificaId { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public int DependenciaId { get; set; }

        public int AreaId { get; set; }

        public int RubroPresupuestalId { get; set; }

        public int DecretoId { get; set; }

    }
}
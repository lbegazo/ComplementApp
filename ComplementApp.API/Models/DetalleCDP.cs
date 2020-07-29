using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TB_DetalleCDP")]
    public class DetalleCDP
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

        public string Contrato { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoTotal { get; set; }


        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoDisponible { get; set; }

        public string Area { get; set; }

        public int Paa { get; set; }

        public int IdSofi { get; set; }

    }
}
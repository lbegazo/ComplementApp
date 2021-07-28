using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPlanAdquisicionHistorico")]
    public class PlanAdquisicionHistorico
    {
        public int PlanAdquisicionHistoricoId { get; set; }
        public int PlanAdquisicioId { get; set; }

        [Column(TypeName = "VARCHAR(500)")]
        public string PlanDeCompras { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal Valor { get; set; }
        [Column(TypeName = "decimal(30,8)")]
        public decimal Saldo { get; set; }
        public bool AplicaContrato { get; set; }
        public long Crp { get; set; }
        public int ActividadGeneralId { get; set; }
        public int ActividadEspecificaId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int DependenciaId { get; set; }
        public int AreaId { get; set; }
        public int RubroPresupuestalId { get; set; }
        public int DecretoId { get; set; }
        public int PciId { get; set; }
        public Pci Pci { get; set; }
        public int EstadoId { get; set; }
        public int UsuarioIdRegistro { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? TransaccionId { get; set; }
    }
}
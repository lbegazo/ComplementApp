using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TPresupuestoFuturoHistorico")]
    public class PresupuestoFuturoHistorico
    {
        [Key]
        public int PresupuestoFuturoHistoricoId { get; set; }

        [Column(TypeName = "VARCHAR(8000)")]
        public string CodigoUNSPSC { get; set; }

        [Column(TypeName = "VARCHAR(8000)")]
        public string Descripcion { get; set; }
        public int MesInicio { get; set; }
        public int MesOferta { get; set; }
        public int DuracionContrato { get; set; }
        public DateTime FechaEstimadaContratacion { get; set; }

        public int ModalidadSeleccionId { get; set; }

        public ModalidadSeleccion ModalidadSeleccion { get; set; }
        public FuenteFinanciacion FuenteFinanciacion { get; set; }
        public int FuenteFinanciacionId { get; set; }      

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorTotal { get; set; }
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorActual { get; set; }
        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorVigenciaFutura { get; set; }
         [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoActual { get; set; }       
        public bool SeRequiereVigenciaFutura { get; set; }
        public int EstadoVigenciaFuturaId { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string UnidadContratacion { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string Ubicacion { get; set; }
        public int UsuarioResponsableId { get; set; }
        public Usuario UsuarioResponsable { get; set; }
        public bool SeAplicaLey { get; set; }
        public bool EsSuministroBYS { get; set; }
        public int DependenciaId { get; set; }
        public Dependencia Dependencia { get; set; }
        public int EstadoActividadPAAId { get; set; }
        public string Observaciones { get; set; }
        public int PciId { get; set; }
        public Pci Pci { get; set; }
        public int UsuarioRegistroId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int PresupuestoFuturoId { get; set; }

    }
}
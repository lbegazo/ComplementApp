using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TDetalleLiquidacion")]
    public class DetalleLiquidacion
    {

        #region PlanPago

        public int DetalleLiquidacionId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(20)")]
        public string NumeroIdentificacion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Contrato { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string Viaticos { get; set; }

        [Required]
        public long Crp { get; set; }
        public int CantidadPago { get; set; }

        [Required]
        public int NumeroPago { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorContrato { get; set; }


        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorAdicionReduccion { get; set; }


        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorCancelado { get; set; }


        [Column(TypeName = "decimal(30,8)")]
        public decimal TotalACancelar { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SaldoActual { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string RubroPresupuestal { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string UsoPresupuestal { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string NombreSupervisor { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string NumeroRadicado { get; set; }
        public DateTime FechaRadicado { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string NumeroFactura { get; set; }

        [Column(TypeName = "VARCHAR(4000)")]
        public string TextoComprobanteContable { get; set; }

        #endregion PlanPago

        #region Formato liquidación

        [Column(TypeName = "decimal(30,8)")]
        public decimal Honorario { get; set; }
        [Column(TypeName = "decimal(30,8)")]
        public decimal HonorarioUvt { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorIva { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ValorTotal { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal TotalRetenciones { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal TotalAGirar { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal BaseSalud { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal AporteSalud { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal AportePension { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal RiesgoLaboral { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal FondoSolidaridad { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ImpuestoCovid { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SubTotal1 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal PensionVoluntaria { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal Afc { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SubTotal2 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal MedicinaPrepagada { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal Dependientes { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal InteresesVivienda { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal TotalDeducciones { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal SubTotal3 { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal RentaExenta { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal LimiteRentaExenta { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal TotalRentaExenta { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal DiferencialRenta { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal BaseGravableRenta { get; set; }

        [Required]
        [Column(TypeName = "decimal(30,8)")]
        public decimal BaseGravableUvt { get; set; }

        public int MesPagoAnterior { get; set; }

        public int MesPagoActual { get; set; }

        #endregion Formato liquidación

        public int PlanPagoId { get; set; }

        public PlanPago PlanPago { get; set; }

        public int ModalidadContrato { get; set; }

        public int UsuarioIdRegistro { get; set; }  

        public DateTime? FechaRegistro { get; set; }   

        public int UsuarioIdModificacion { get; set; }  

        public DateTime? FechaModificacion { get; set; }        

        public ICollection<LiquidacionDeduccion> Deducciones { get; set; }

        public bool? BaseImpuestos { get; set; }

        [Column(TypeName = "decimal(30,8)")]
        public decimal ViaticosPagados { get; set; }

        public int MesSaludAnterior { get; set; }

        public int MesSaludActual { get; set; }

        public int TerceroId { get; set; }

        public bool Procesado { get; set; }

        public long? Obligacion { get; set; }

        public long? OrdenPago { get; set; }

        public DateTime? FechaOrdenPago { get; set; }

        public int? DiasAlPago { get; set; }

        public ICollection<DetalleArchivoLiquidacion> DetalleArchivo { get; set; }
    }
}
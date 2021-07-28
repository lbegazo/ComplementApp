using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ComplementApp.API.Models
{
    [Table("TSolicitudCDP")]
    public class SolicitudCDP
    {
        public int SolicitudCDPId { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Column(TypeName = "VARCHAR(150)")]
        public string EstadoCDP { get; set; }

        public long? Cdp { get; set; }

        [Required]
        public int NumeroActividad { get; set; }

        [Required]
        public bool AplicaContrato { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(500)")]
        public string NombreBienServicio { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(500)")]
        public string ProyectoInversion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(500)")]
        public string ActividadProyectoInversion { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(500)")]
        public string ObjetoBienServicioContratado { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(500)")]
        public string Observaciones { get; set; }

        public TipoDetalleCDP TipoDetalleCDP { get; set; }

        public int? TipoDetalleCDPId { get; set; }

        public TipoOperacion TipoOperacion { get; set; }

        [Required]
        public int TipoOperacionId { get; set; }

        public Estado EstadoSolicitudCDP { get; set; }

        public int EstadoSolicitudCDPId { get; set; }

        public ICollection<DetalleSolicitudCDP> DetalleSolicitudCDPs { get; set; }

        public Usuario Usuario { get; set; }

        public int UsuarioId { get; set; }

        public int? PciId { get; set; }
        public Pci Pci { get; set; }

        //**************Auditoría**********//

        [Required]
        public int UsuarioIdRegistro { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        public int UsuarioIdModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }



    }
}
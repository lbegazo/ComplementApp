using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComplementApp.API.Models
{
    [Table("TRelacionContable")]
    public class RelacionContable
    {
        public int RelacionContableId { get; set; }

        public int? RubroPresupuestalId { get; set; }

        public int? CuentaContableId { get; set; }

        public CuentaContable CuentaContable { get; set; }

        public int? AtributoContableId { get; set; }

        public AtributoContable AtributoContable { get; set; }

        public int? TipoGastoId { get; set; }

        public TipoGasto TipoGasto { get; set; }

        public int? TipoOperacion { get; set; }
        public int? UsoContable { get; set; }
        public int? PciId { get; set; }
        public Pci Pci { get; set; }
        public bool Estado { get; set; }
    }
}
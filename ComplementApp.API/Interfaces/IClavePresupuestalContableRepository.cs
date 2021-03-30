using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IClavePresupuestalContableRepository
    {
        Task<PagedList<CDPDto>> ObtenerCompromisosParaClavePresupuestalContable(int tipo, int? terceroId, UserParams userParams);
        Task<ICollection<ClavePresupuestalContableDto>> ObtenerRubrosPresupuestalesXCompromiso(int crp, int pciId);
        Task<ICollection<ClavePresupuestalContableDto>> ObtenerClavesPresupuestalContableXCompromiso(int crp, int pci);
        Task<ICollection<RelacionContableDto>> ObtenerRelacionesContableXRubroPresupuestal(int rubroPresupuestalId, int pciId);
        Task<ICollection<ValorSeleccion>> ObtenerUsosPresupuestalesXRubroPresupuestal(int rubroPresupuestalId);
        Task<bool> RegistrarRelacionContable(RelacionContable relacion);
        Task<ICollection<ClavePresupuestalContableDto>> ObtenerListaClavePresupuestalContable(int pciId);
        Task<ClavePresupuestalContable> ObtenerClavePresupuestalContableBase(int claveId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IClavePresupuestalContableRepository
    {
        Task<PagedList<CDPDto>> ObtenerCompromisosParaClavePresupuestalContable(int? terceroId, int? numeroCrp, UserParams userParams);
        Task<ICollection<ClavePresupuestalContableDto>> ObtenerRubrosPresupuestalesXCompromiso(int cdpId);
        Task<ICollection<RelacionContableDto>> ObtenerRelacionesContableXRubroPresupuestal(int rubroPresupuestalId);
        Task<ICollection<ValorSeleccion>> ObtenerUsosPresupuestalesXRubroPresupuestal(int rubroPresupuestalId);
        Task<bool> RegistrarRelacionContable(RelacionContable relacion);
    }
}
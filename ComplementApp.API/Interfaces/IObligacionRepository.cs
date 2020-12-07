using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IObligacionRepository
    {
        Task<PagedList<CDPDto>> ObtenerCompromisosParaClavePresupuestalContable(int? terceroId, int? numeroCrp, UserParams userParams);
        Task<ICollection<ClavePresupuestalContableDto>> ObtenerRubrosParaClavePresupuestalContable(int cdpId);
        Task<ICollection<RelacionContableDto>> ObtenerRelacionesContableXRubro(int rubroPresupuestalId);
        Task<bool> RegistrarRelacionContable(RelacionContable relacion);
    }
}
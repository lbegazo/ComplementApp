using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IPlanAdquisicionRepository
    {
        Task<ICollection<PlanAdquisicionDto>> ObtenerListaPlanAnualAdquisicion(int pciId);
        Task<PlanAdquisicion> ObtenerPlanAnualAdquisicionBase(int id);
        Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAnualAdquisicionPaginada(int usuarioId, int esCreacion, int? rubroPresupuestalId,
                                                                                int? numeroCdp, UserParams userParams);
        Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAnualAdquisicionSinCDP(int usuarioId, int? rubroPresupuestalId, UserParams userParams);
        Task<List<DetalleCDPDto>> ObtenerListaPlanAdquisicionSinCDPXIds(List<int> listaId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using ComplementApp.API.Helpers;

namespace ComplementApp.API.Data
{
    public interface IPlanPagoRepository
    {
        Task<PagedList<PlanPago>> ObtenerListaPlanPago(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<PlanPago> ObtenerPlanPago(int planPagoId);

        Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId);

        Task<PlanPago> ObtenerPlanPagoDetallado(int planPagoId);
    }
}
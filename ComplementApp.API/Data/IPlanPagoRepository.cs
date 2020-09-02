using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IPlanPagoRepository
    {
        Task<IEnumerable<PlanPago>> ObtenerListaPlanPago(int terceroId, List<int> listaEstadoId);

        Task<PlanPago> ObtenerPlanPago(int planPagoId);

        Task<DetallePlanPago> ObtenerDetallePlanPago(int planPagoId);
    }
}
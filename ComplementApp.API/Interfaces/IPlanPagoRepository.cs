using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using ComplementApp.API.Helpers;

namespace ComplementApp.API.Interfaces
{
    public interface IPlanPagoRepository
    {
        Task<PagedList<PlanPago>> ObtenerListaPlanPago(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<PagedList<PlanPago>> ObtenerListaPlanPagoXCompromiso(long crp, List<int> listaEstadoId, UserParams userParams);

        Task<PlanPago> ObtenerPlanPagoBase(int planPagoId);

        Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId);

        Task<ICollection<DetallePlanPagoDto>> ObtenerListaDetallePlanPagoXIds(List<int> listaPlanPagoId);

        Task<PlanPago> ObtenerPlanPagoDetallado(int planPagoId);

        int ObtenerCantidadMaximaPlanPago(long crp);

        Task RegistrarPlanPago(PlanPago plan);

        void ActualizarPlanPago(PlanPago plan);

        Task<ICollection<RadicadoDto>> ObtenerListaRadicado(int mes, int? terceroId, List<int> listaEstadoId);

        Task<PagedList<RadicadoDto>> ObtenerListaRadicadoPaginado(int mes, int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<List<PlanPago>> ObtenerListaPlanPagoXIds(List<int> listaPlanPagoId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using ComplementApp.API.Helpers;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IPlanPagoRepository
    {
        Task<PagedList<PlanPago>> ObtenerListaPlanPago(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<PagedList<PlanPago>> ObtenerListaPlanPagoXCompromiso(long crp, List<int> listaEstadoId, UserParams userParams);

        Task<PlanPago> ObtenerPlanPagoBase(int planPagoId);

        Task<DetallePlanPagoDto> ObtenerDetallePlanPago(int planPagoId);

        Task<DetallePlanPagoDto> ObtenerDetallePlanPagoParaSolicitudPago(int planPagoId);

        Task<ICollection<DetallePlanPagoDto>> ObtenerListaDetallePlanPagoXIds(List<int> listaSolicitudPagoId);

        Task<PlanPago> ObtenerPlanPagoDetallado(int planPagoId);

        int ObtenerCantidadMaximaPlanPago(long crp, int pciId);

        Task<ICollection<DetallePlanPagoDto>> ObtenerListaCantidadMaximaPlanPago(List<long> compromisos, int pciId);

        Task RegistrarPlanPago(PlanPago plan);

        void ActualizarPlanPago(PlanPago plan);

        Task<ICollection<RadicadoDto>> ObtenerListaRadicado(int pciId, int mes, int? terceroId, List<int> listaEstadoId);

        Task<PagedList<RadicadoDto>> ObtenerListaRadicadoPaginado(int pciId, int mes, int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<ICollection<PlanPagoDto>> ObtenerListaPlanPagoTotal(int pciId);

        Task<int> CantidadPlanPagoParaCompromiso(long crp, int pcidId);
        List<int> ObtenerListaPlanPagoParaCompromiso(List<long> listaCrp, int pcidId);

        #region Forma Pago Compromiso
        Task<PagedList<CDPDto>> ObtenerCompromisosSinPlanPago(int? terceroId, int? numeroCrp, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerCompromisosConPlanPago(int? terceroId, int? numeroCrp, UserParams userParams);
        Task<ICollection<LineaPlanPagoDto>> ObtenerLineasPlanPagoXCompromiso(int numeroCrp, int pci);
        //Task<PlanPago> ObtenerUltimoPlanPagoDeCompromisoXMes(int crp, int MesId);

        #endregion Forma Pago Compromiso
    }
}
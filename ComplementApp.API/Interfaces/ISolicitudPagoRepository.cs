using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface ISolicitudPagoRepository
    {
        Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId, int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int cdpId);
        Task<ICollection<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp);
        Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId);
        Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId);
        Task<ICollection<FormatoSolicitudPago>> ObtenerListaSolicitudPagoXPlanPagoIds(List<int> planPagoIds);
        Task<FormatoSolicitudPago> ObtenerSolicitudPagoXPlanPagoId(int planPagoId);

    }
}
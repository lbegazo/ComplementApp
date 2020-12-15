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
        Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId, int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int cdpId);
        Task<ICollection<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp);
        Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId);
        Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId);
    }
}
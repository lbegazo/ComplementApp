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
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int crp, int pciId);
        Task<ICollection<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp, int pciId);
        Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerListaSolicitudPagoAprobada(int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId);
        Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId);

        Task<Numeracion> ObtenerUltimaNumeracionDisponible(int pciId);
        Task<Numeracion> ObtenerNumeracionBase(int numeracionId);
        Task<Numeracion> ObtenerNumeracionxNumeroFactura(string numeroFactura);

        #region Proceso Liquidación Masiva
        Task<ICollection<FormatoSolicitudPago>> ObtenerListaFormatoSolicitudPagoBase(List<int> listaSolicitudPagoId);
        Task<List<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoXId(List<int> listaSolicitudPagoId);
        Task<PagedList<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoPaginada(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        #endregion Proceso Liquidación Masiva
    }
}
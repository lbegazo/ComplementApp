using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface ISolicitudPagoRepository
    {
        Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId, int? terceroId, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerListaCompromisoConContrato(int usuarioId, string numeroContrato,
                                                                                long? crp, int? terceroId,
                                                                                UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(long crp, int pciId);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXCompromiso(long crp);
        Task<List<CDPDto>> ObtenerInformacionFinancieraXListaCompromiso(string numeroContrato, List<long> listaCrp);
        Task<List<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp, int pciId);
        Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerListaSolicitudPagoAprobada(int? terceroId, UserParams userParams);
        Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId);
        Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId);
        Task<DetalleFormatoSolicitudPago> ObtenerDetalleFormatoSolicitudPagoBase(int detalleFormatoSolicitudPagoId);

        Task<List<DetalleFormatoSolicitudPago>> ObtenerDetalleSolicitudPagoLiquidacionIds(List<int> listaLiquidacionId);

        Task<Numeracion> ObtenerUltimaNumeracionDisponible(int pciId);
        Task<Numeracion> ObtenerNumeracionBase(int numeracionId);
        Task<Numeracion> ObtenerNumeracionxNumeroFactura(string numeroFactura);
        Task<List<long>> ObtenerListaCompromisoXNumeroContrato(string numeroContrato);
        Task<List<CDPDto>> ObtenerPagosRealizadosXListaCompromiso(List<long> listaCrp);

        #region Proceso Liquidación Masiva
        Task<ICollection<FormatoSolicitudPago>> ObtenerListaFormatoSolicitudPagoBase(List<int> listaSolicitudPagoId);
        Task<List<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoXId(List<int> listaSolicitudPagoId);
        Task<PagedList<FormatoSolicitudPagoDto>> ObtenerListaSolicitudPagoPaginada(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        #endregion Proceso Liquidación Masiva
    }
}
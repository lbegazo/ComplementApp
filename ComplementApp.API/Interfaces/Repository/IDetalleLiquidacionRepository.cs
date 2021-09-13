using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IDetalleLiquidacionRepository
    {
        Task RegistrarDetalleLiquidacion(DetalleLiquidacion detalleLiquidacion);

        Task<FormatoCausacionyLiquidacionPagos> ObtenerDetalleFormatoCausacionyLiquidacionPago(long detalleLiquidacionId);

        Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesParaCuentaPorPagarArchivo(int? terceroId,
                List<int> listaEstadoId,
                bool? procesado, UserParams userParams);

        Task<List<int>> ObtenerListaDetalleLiquidacionTotal(int pcidId, int? terceroId, List<int> listaEstadoId, bool? procesado);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosAnterior(long terceroId, int pciId);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionAnterior(int terceroId, int pciId);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(List<int> listaTerceroId, int pciId);

        ICollection<DetalleLiquidacion> ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(List<int> listaTerceroId, int pciId);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId, int pciId);

        bool RegistrarArchivoDetalleLiquidacion(ArchivoDetalleLiquidacion archivo);

        bool RegistrarDetalleArchivoLiquidacion(List<DetalleArchivoLiquidacion> listaDetalle);

        int ObtenerUltimoConsecutivoArchivoLiquidacion(int pciId);

        #region Liquidación Masiva

        bool RegistrarListaDetalleLiquidacion(IList<DetalleLiquidacion> listaDetalleLiquidacion);

        #endregion Liquidación Masiva

        #region Archivo Obligacion Presupuestal

        Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesParaObligacionArchivo(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado,
                    UserParams userParams);

        Task<bool> ValidarLiquidacionSinClavePresupuestal(
                                   int? terceroId,
                                   List<int> listaEstadoId, int pciId);

        Task<List<int>> ObtenerLiquidacionIdsParaArchivoObligacion(int pciId,
                                                                    int? terceroId,
                                                                    List<int> listaEstadoId,
                                                                    bool? procesado);
        Task<List<int>> ObtenerLiquidacionesConClaveParaArchivoObligacion(int pciId, List<int> listaLiquidacionId);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerCabeceraParaArchivoObligacion(List<int> listaLiquidacionId, int pciId);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionSinClavePresupuestalXIds(List<int> listaLiquidacionId);

        Task<ICollection<ClavePresupuestalContableParaArchivo>> ObtenerItemsLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<ICollection<DeduccionDetalleLiquidacionParaArchivo>> ObtenerDeduccionesLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerUsosParaArchivoObligacion(List<int> listaLiquidacionId);
        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerFacturaParaArchivoObligacion(List<int> listaLiquidacionId);
        Task<List<int>> ObtenerLiquidacionIdsConUsosPresupuestales(List<int> listaLiquidacionId);
        Task<List<int>> ObtenerLiquidacionIdsConRubroFuncionamiento(List<int> listaLiquidacionId);

        #endregion Archivo Obligacion Presupuestal
    }
}
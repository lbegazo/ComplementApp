using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IDetalleLiquidacionRepository
    {
        Task RegistrarDetalleLiquidacion(DetalleLiquidacion detalleLiquidacion);

        Task<FormatoCausacionyLiquidacionPagos> ObtenerDetalleFormatoCausacionyLiquidacionPago(long detalleLiquidacionId);

        Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerListaDetalleLiquidacion(int? terceroId,
                List<int> listaEstadoId,
                bool? procesado, UserParams userParams);

        Task<List<int>> ObtenerListaDetalleLiquidacionTotal(int? terceroId, List<int> listaEstadoId, bool? procesado);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosAnterior(long terceroId);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionAnterior(int terceroId);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(List<int> listaTerceroId);

        ICollection<DetalleLiquidacion> ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(List<int> listaTerceroId);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId);

        bool RegistrarArchivoDetalleLiquidacion(ArchivoDetalleLiquidacion archivo);

        bool RegistrarDetalleArchivoLiquidacion(List<DetalleArchivoLiquidacion> listaDetalle);

        int ObtenerUltimoConsecutivoArchivoLiquidacion();

        #region Liquidación Masiva

        bool RegistrarListaDetalleLiquidacion(IList<DetalleLiquidacion> listaDetalleLiquidacion);

        #endregion Liquidación Masiva

        #region Archivo Obligacion Presupuestal

        Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesParaObligacionArchivo(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado,
                    UserParams userParams);

        Task<List<int>> ObtenerLiquidacionIdsParaArchivoObligacion(int? terceroId,
                                                                    List<int> listaEstadoId,
                                                                    bool? procesado);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerCabeceraParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<ICollection<ClavePresupuestalContableParaArchivo>> ObtenerItemsLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<ICollection<DeduccionDetalleLiquidacionParaArchivo>> ObtenerDeduccionesLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerUsosParaArchivoObligacion(List<int> listaLiquidacionId);

        Task<List<int>> ObtenerLiquidacionIdsConUsosPresupuestales(List<int> listaLiquidacionId);

        #endregion Archivo Obligacion Presupuestal
    }
}
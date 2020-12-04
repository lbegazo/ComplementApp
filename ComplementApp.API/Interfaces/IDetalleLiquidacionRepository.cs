using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
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

        Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId);

        bool RegistrarArchivoDetalleLiquidacion(ArchivoDetalleLiquidacion archivo);

        bool RegistrarDetalleArchivoLiquidacion(List<DetalleArchivoLiquidacion> listaDetalle);

        int ObtenerUltimoConsecutivoArchivoLiquidacion();

        Task<ICollection<ActividadEconomica>> ObtenerListaActividadesEconomicaXTercero(int terceroId);
    }
}
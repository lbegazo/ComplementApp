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

        Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerListaDetalleLiquidacion(int? terceroId, List<int> listaEstadoId, UserParams userParams);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion);

        Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosAnterior(long terceroId);

        Task<DetalleLiquidacion> ObtenerDetalleLiquidacionAnterior(int terceroId);
    }
}
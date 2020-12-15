using System.Threading.Tasks;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoLiquidacionSolicitudPago
    {
         Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoSolicitudPago(int planPagoId, decimal valorBaseCotizacion, int? actividadEconomicaId);

    }
}
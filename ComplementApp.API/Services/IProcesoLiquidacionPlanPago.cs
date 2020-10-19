using System.Threading.Tasks;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Services
{
    public interface IProcesoLiquidacionPlanPago
    {
        Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacionyLiquidacionPago(int planPagoId, decimal valorBaseGravable);
        
    }
}
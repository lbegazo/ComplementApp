using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoLiquidacionPlanPago
    {
        Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacionyLiquidacionPago(int planPagoId, decimal valorBaseGravable, int? actividadEconomicaId);

        Task<bool> RegistrarListaDetalleLiquidacion(int usuarioId, string listaPlanPagoId, List<int> listIds, bool esSeleccionarTodo, int? terceroId);
    }
}
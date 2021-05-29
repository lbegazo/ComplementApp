using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoLiquidacionPlanPago
    {
        Task<FormatoCausacionyLiquidacionPagos> ObtenerFormatoCausacionyLiquidacionPago(int solicitudPagoId, int planPagoId, int pciId, decimal valorBaseGravable, int? actividadEconomicaId);

        Task<bool> RegistrarListaDetalleLiquidacion(int usuarioId, int pciId, string listaPlanPagoId, List<int> listIds, bool esSeleccionarTodo, int? terceroId);
    }
}
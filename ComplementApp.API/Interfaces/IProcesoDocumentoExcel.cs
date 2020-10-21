using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using Microsoft.AspNetCore.Http;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoDocumentoExcel
    {
        bool EliminarInformacionCDP();

        DataTable ObtenerDetalleDeExcel(IFormFile file);

        DataTable ObtenerCabeceraDeExcel(IFormFile file);

        DataTable ObtenerPlanPagosDeExcel(IFormFile file);

        Task<string> LeerInformacionDelServicio();

        List<CDPDto> obtenerListaDeCDP(DataTable dtCabecera);

        List<DetalleCDPDto> obtenerListaDeDetalleCDP(DataTable dtDetalle);

        List<PlanPagoDto> obtenerListaDePlanPago(DataTable dtPlanPago);

    }
}
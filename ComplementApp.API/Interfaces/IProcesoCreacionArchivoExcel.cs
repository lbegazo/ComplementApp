using System.Collections.Generic;
using System.Data;
using ComplementApp.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoCreacionArchivoExcel
    {
        DataTable ObtenerTablaDeListaRadicado(List<RadicadoDto> lista);
        DataTable ObtenerTablaDeListaTercero(List<TerceroDto> lista);
        DataTable ObtenerTablaDeListaClavePresupuestalContable(List<ClavePresupuestalContableDto> lista);
        DataTable ObtenerTablaDeListaPlanPago(List<PlanPagoDto> lista);
        DataTable ObtenerTablaDeListaContrato(List<ContratoDto> lista);
        DataTable ObtenerTablaDeListaTerceroDeduccion(List<TerceroDeduccionDto> lista);
        DataTable ObtenerTablaDeListaParametroLiquidacionTercero(List<ParametroLiquidacionTerceroDto> lista);
        DataTable ObtenerTablaDeListaActividadEconomica(List<ValorSeleccion> lista);
        DataTable ObtenerTablaDeListaPlanAnualAdquisicion(List<DetalleCDPDto> lista);
        DataTable ObtenerTablaDetallePlanAnualAdquisicion(List<CDPDto> lista);
        FileStreamResult ExportExcel(HttpResponse response, DataTable dt, string nombreArchivo);
    }
}
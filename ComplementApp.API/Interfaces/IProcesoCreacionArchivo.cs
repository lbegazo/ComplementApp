using System.Collections.Generic;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;

namespace ComplementApp.API.Interfaces
{
    public interface IProcesoCreacionArchivo
    {
        #region Cuenta Por Pagar
        string ObtenerInformacionMaestroLiquidacion_ArchivoCuentaPagar(List<DetalleLiquidacionParaArchivo> lista);
        string ObtenerInformacionDetalleLiquidacion_ArchivoCuentaPagar(List<DetalleLiquidacionParaArchivo> lista);

        #endregion Cuenta Por Pagar

        #region Obligacion
        string ObtenerInformacionCabeceraLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> lista);
        string ObtenerInformacionItemsLiquidacion_ArchivoObligacion(List<ClavePresupuestalContableParaArchivo> lista);
        string ObtenerInformacionUsosLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> lista);
        string ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(List<DeduccionDetalleLiquidacionParaArchivo> lista);

        #endregion Obligacion
    }
}
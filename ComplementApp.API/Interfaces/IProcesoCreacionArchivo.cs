using System;
using System.Collections.Generic;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Models;

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
        string ObtenerInformacionUsosLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> lista, List<ClavePresupuestalContableParaArchivo> listaRubros);
        string ObtenerInformacionDeduccionesLiquidacion_ArchivoObligacion(List<int> liquidacionIds, List<DeduccionDetalleLiquidacionParaArchivo> lista);
        string ObtenerInformacionFacturaLiquidacion_ArchivoObligacion(List<DetalleLiquidacionParaArchivo> lista);

        #endregion Obligacion

        #region Registrar archivo

        ArchivoDetalleLiquidacion RegistrarArchivoDetalleLiquidacion(int usuarioId, int pciId, List<int> listIds,
                                                                               string nombreArchivo, int consecutivo,
                                                                               int tipoDocumentoArchivo);

        string ObtenerNombreArchivo(DateTime fecha, int consecutivo, int tipoDocumentoArchivo, int tipoArchivo);

        void RegistrarDetalleArchivoLiquidacion(int archivoId, List<int> listIds);

        #endregion Registrar archivo
    }
}
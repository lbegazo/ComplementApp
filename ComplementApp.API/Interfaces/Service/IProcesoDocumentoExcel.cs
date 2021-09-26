using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models.ExcelDocumento;
using Microsoft.AspNetCore.Http;

namespace ComplementApp.API.Interfaces.Service
{
    public interface IProcesoDocumentoExcel
    {
        #region Carga Masiva PAA

        bool EliminarInformacionCDP();

        DataTable ObtenerDetalleDeExcel(IFormFile file);

        DataTable ObtenerCabeceraDeExcel(IFormFile file);

        DataTable ObtenerPlanPagosDeExcel(IFormFile file);

        Task<string> LeerInformacionDelServicio();

        List<CDPDto> obtenerListaDeCDP(DataTable dtCabecera);

        List<DetalleCDPDto> obtenerListaDeDetalleCDP(DataTable dtDetalle);

        List<PlanPagoDto> obtenerListaDePlanPago(DataTable dtPlanPago);

        #endregion Carga Masiva PAA

        #region Carga Registro Gestion Presupuestal

        DataTable ObtenerInformacionDocumentoExcel(IFormFile file, int numeroColumna);

        List<DocumentoCdp> obtenerListaDocumentoCdp(DataTable dtCabecera);
        List<DocumentoCompromiso> obtenerListaDocumentoCompromiso(DataTable dtCabecera);
        List<DocumentoObligacion> obtenerListaDocumentoObligacion(DataTable dtCabecera);
        List<DocumentoOrdenPago> obtenerListaDocumentoOrdenPago(DataTable dtCabecera);

        #endregion Carga Registro Gestion Presupuestal

    }
}
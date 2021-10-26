using System.Collections.Generic;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models.ExcelDocumento;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IDocumentoRepository
    {
        bool InsertaCabeceraCDP(IList<CDPDto> listaCdp);

        bool InsertaDetalleCDP(IList<DetalleCDPDto> lista);

        bool InsertaCabeceraCDPConTercero(IList<CDPDto> lista);

        bool InsertaPlanDePago(IList<PlanPagoDto> lista);

        bool EliminarCabeceraCDP();

        bool EliminarDetalleCDP();

        bool EliminarCabeceraCDPXInstancia(int instancia);

        #region Carga Registro Gestion Presupuestal

        bool EliminarDocumentoCDP();

        bool EliminarDocumentoCompromiso();

        bool EliminarDocumentoObligacion();

        bool EliminarDocumentoOrdenPago();

        bool InsertarListaDocumentoCDP(IList<DocumentoCdp> listaCdp);

        bool InsertarListaDocumentoCompromiso(IList<DocumentoCompromiso> lista);

        bool InsertarListaDocumentoObligacion(IList<DocumentoObligacion> lista);

        bool InsertarListaDocumentoOrdenPago(IList<DocumentoOrdenPago> lista);

        #endregion Carga Registro Gestion Presupuestal
    }
}
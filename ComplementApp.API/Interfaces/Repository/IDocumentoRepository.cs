using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
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

        bool EliminarDatosDocumentoCDP();

        bool InsertarListaDocumentoCDP(IList<DocumentoCdp> listaCdp);
    }
}
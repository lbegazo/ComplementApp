using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IDocumentoRepository
    {
        bool InsertaCabeceraCDP(IList<CDPDto> lista);

        bool InsertaDetalleCDP(IList<DetalleCDPDto> lista);

        bool InsertaCabeceraCDPConTercero(IList<CDPDto> lista);

        bool InsertaPlanDePago(IList<PlanPagoDto> lista);

        bool EliminarCabeceraCDP();        

        bool EliminarDetalleCDP();

        bool EliminarCabeceraCDPXInstancia(int instancia);
    }
}
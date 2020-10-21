using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IDocumentoRepository
    {
        bool InsertaCabeceraCDP(IList<CDPDto> listaCdp, IList<DetalleCDPDto> listaDetalle);

        bool InsertaDetalleCDP(IList<DetalleCDPDto> lista);

        bool InsertaCabeceraCDPConTercero(IList<CDPDto> lista);

        bool InsertaPlanDePago(IList<PlanPagoDto> lista);

        bool EliminarCabeceraCDP();        

        bool EliminarDetalleCDP();

        bool EliminarCabeceraCDPXInstancia(int instancia);
    }
}
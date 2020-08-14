using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IDocumentoRepository
    {
        // Task<bool> InsertaCabeceraCDP(IList<CDP> lista);

        // Task<bool> InsertaDetalleCDP(IList<DetalleCDP> lista);

        bool InsertaCabeceraCDP(IList<CDPDto> lista);

        bool InsertaDetalleCDP(IList<DetalleCDPDto> lista);

        bool EliminarCabeceraCDP();

        bool EliminarDetalleCDP();
    }
}
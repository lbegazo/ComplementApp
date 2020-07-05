using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IDocumentoRepository
    {
        Task InsertaCabeceraCDP(IList<CDP> lista);

        Task InsertaDetalleCDP(IList<DetalleCDP> lista);

        bool EliminarCabeceraCDP();

        bool EliminarDetalleCDP();
    }
}
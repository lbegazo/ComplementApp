using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface ICDPRepository
    {
        Task<IEnumerable<CDP>> ObtenerCDPsXFiltro(int numeroCDP);

        Task<IEnumerable<DetalleCDP>> ObtenerItemsCDPxFiltro(string usuario, int numeroCDP);
    }
}
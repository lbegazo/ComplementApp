using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface ICDPRepository
    {
        Task<IEnumerable<CDP>> ObtenerListaCDP(string usuario);

        Task<CDP> ObtenerCDP(string usuario, int numeroCDP);

        Task<IEnumerable<DetalleCDP>> ObtenerDetalleDeCDP(string usuario, int numeroCDP);
    }
}
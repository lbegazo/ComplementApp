using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Data
{
    public interface ICDPRepository
    {
        Task<IEnumerable<CDP>> ObtenerListaCDP(string usuario);

        Task<CDP> ObtenerCDP(string usuario, int numeroCDP);

        Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(string usuario, int numeroCDP);
    }
}
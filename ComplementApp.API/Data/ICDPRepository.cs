using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Data
{
    public interface ICDPRepository
    {
        Task<IEnumerable<CDP>> ObtenerListaCDP(int usuarioId);

        Task<CDPDto> ObtenerCDP(int usuarioId, int numeroCDP);

        Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(int usuarioId, int numeroCDP);
    }
}
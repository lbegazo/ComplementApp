using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
namespace ComplementApp.API.Interfaces.Repository
{
    public interface ICDPRepository
    {
        Task<PagedList<CDP>> ObtenerListaCompromiso(UserParams userParams);
        Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesPorCompromiso(long crp, int pciId);
        Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesXNumeroContrato(string numeroContrato);
        Task<CDPDto> ObtenerCDPPorCompromiso(long crp);
        Task<PagedList<CDPDto>> ObtenerDetallePlanAnualAdquisicion(long cdp, int instancia, UserParams userParams);
        Task<PagedList<CDPDto>> ObtenerListaCdpParaVinculacion(long? cdp, int instancia, UserParams userParams);
        
    }
}
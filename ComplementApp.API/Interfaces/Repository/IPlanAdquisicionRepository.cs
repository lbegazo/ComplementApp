using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IPlanAdquisicionRepository
    {
        Task<ICollection<DetalleCDP>> ObtenerListaPlanAnualAdquisicion(int pciId);
        Task<DetalleCDP> ObtenerPlanAnualAdquisicionBase(int id);
    }
}
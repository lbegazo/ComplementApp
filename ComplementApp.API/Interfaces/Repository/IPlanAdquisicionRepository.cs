using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IPlanAdquisicionRepository
    {
        Task<ICollection<PlanAdquisicion>> ObtenerListaPlanAnualAdquisicion(int pciId);
        Task<PlanAdquisicion> ObtenerPlanAnualAdquisicionBase(int id);
    }
}
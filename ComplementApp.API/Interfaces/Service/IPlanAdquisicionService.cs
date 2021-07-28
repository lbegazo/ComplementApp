using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Service
{
    public interface IPlanAdquisicionService
    {
        Task ActualizarPlanAdquisicion(int pciId, int transaccionId, PlanAdquisicion planAdquisicion);

        Task RegistrarPlanAdquisicionHistorico(PlanAdquisicion planAdquisicion, decimal valor, int transaccionId, bool esDebito);

        Task ActualizarPlanAdquisicionExterno(int usuarioId, int transaccionId, int planAdquisicionId, string objetoBien, decimal valor, bool esDebito);
    }
}
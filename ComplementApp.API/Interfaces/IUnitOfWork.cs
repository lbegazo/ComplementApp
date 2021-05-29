using System.Threading.Tasks;
using ComplementApp.API.Interfaces.Repository;

namespace ComplementApp.API.Interfaces
{
    public interface IUnitOfWork
    {
        IPlanPagoRepository PlanPagoRepository {get; }

        Task<bool> CompleteAsync();

        bool Complete();

        void Rollback();

        Task RollbackAsync();
    }
}
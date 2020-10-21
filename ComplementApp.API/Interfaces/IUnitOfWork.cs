using System.Threading.Tasks;


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
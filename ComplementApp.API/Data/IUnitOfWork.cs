using System.Threading.Tasks;


namespace ComplementApp.API.Data
{
    public interface IUnitOfWork
    {
        Task<bool> CompleteAsync();
    }
}
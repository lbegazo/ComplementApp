namespace ComplementApp.API.Interfaces.Repository
{
    public interface IBaseRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
    }
}
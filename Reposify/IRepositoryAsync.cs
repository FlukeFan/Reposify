using System.Threading.Tasks;

namespace Reposify
{
    public interface IRepositoryAsync
    {
        Task<T> SaveAsync<T>(T entity)      where T : class, IEntity;
        Task<T> LoadAsync<T>(object id)     where T : class, IEntity;
        Task    DeleteAsync<T>(T entity)    where T : class, IEntity;
    }
}

using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbLinqExecutorAsync
    {
        Task<TResult> ExecuteAsync<TEntity, TResult>(IDbLinqAsync<TEntity, TResult> query) where TEntity : class;
    }
}

using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbLinqExecutorAsync
    {
        Task<TResult> ExecuteAsync<TEntity, TResult>(IDbLinq<TEntity, TResult> query) where TEntity : class;
    }
}

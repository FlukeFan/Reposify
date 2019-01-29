using System.Linq;
using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbLinqAsync<TEntity, TResult>
    {
        Task<TResult> ExecuteAsync(IQueryable<TEntity> queryable);
    }
}

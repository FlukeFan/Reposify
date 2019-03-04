using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbLinqExecutorAsync
    {
        Task<List<TEntity>> ListAsync<TEntity>(IDbLinq<TEntity> query) where TEntity : class;
    }
}

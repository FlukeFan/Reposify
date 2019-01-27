using System.Linq;

namespace Reposify
{
    public interface IQueryableRepository
    {
        TResult         Execute<TEntity, TResult>(IDbLinq<TEntity, TResult> query);
        IQueryable<T>   Query<T>();
    }
}

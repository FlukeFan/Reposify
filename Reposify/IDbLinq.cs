using System.Linq;

namespace Reposify
{
    public interface IDbLinq<TEntity, TResult>
    {
        TResult Execute(IQueryable<TEntity> queryable);
    }
}

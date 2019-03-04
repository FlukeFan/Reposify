using System.Linq;

namespace Reposify
{
    public interface IDbLinq<TEntity>
    {
        IQueryable<TEntity> Prepare(IQueryable<TEntity> queryable);
    }
}

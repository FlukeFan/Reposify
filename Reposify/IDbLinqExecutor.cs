using System.Collections.Generic;

namespace Reposify
{
    public interface IDbLinqExecutor
    {
        List<TEntity>   List<TEntity>(IDbLinq<TEntity> query) where TEntity : class;
        long            Count<TEntity>(IDbLinq<TEntity> query) where TEntity : class;
    }
}

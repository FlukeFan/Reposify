using System.Collections.Generic;
using Reposify.Queries;

namespace Reposify
{
    public interface IRepository<TId>
    {
        T               Save<T>(T entity)               where T : IEntity<TId>;
        T               Load<T>(TId id)                 where T : IEntity<TId>;
        void            Delete<T>(T entity)             where T : IEntity<TId>;
        void            Flush();

        Query<T, TId>   Query<T>()                      where T : IEntity<TId>;
        IList<T>        Satisfy<T>(Query<T, TId> query) where T : IEntity<TId>;
    }
}

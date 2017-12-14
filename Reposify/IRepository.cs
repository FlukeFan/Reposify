using System;
using System.Collections.Generic;
using Reposify.Queries;

namespace Reposify
{
    public interface IRepository<TId> : IDisposable
    {
        void            Execute(IDbExecution dbExecution);
        T               Execute<T>(IDbQuery<T> dbQuery);

        T               Save<T>(T entity)                               where T : class, IEntity<TId>;
        T               Load<T>(TId id)                                 where T : class, IEntity<TId>;
        void            Delete<T>(T entity)                             where T : class, IEntity<TId>;
        void            Flush();

        Query<T, TId>   Query<T>()                                      where T : class, IEntity<TId>;
        IList<T>        Satisfy<T>(Query<T, TId> query)                 where T : class, IEntity<TId>;
    }
}

using System;
using System.Collections.Generic;
using Reposify.Queries;

namespace Reposify
{
    public interface IRepository : IDisposable
    {
        void            Execute(IDbExecution dbExecution);
        T               Execute<T>(IDbQuery<T> dbQuery);

        T               Save<T>(T entity)                   where T : class, IEntity;
        T               Load<T>(object id)                  where T : class, IEntity;
        void            Delete<T>(T entity)                 where T : class, IEntity;
        void            Flush();

        Query<T>        Query<T>()                          where T : class, IEntity;
        IList<T>        Satisfy<T>(Query<T> query)          where T : class, IEntity;
    }
}

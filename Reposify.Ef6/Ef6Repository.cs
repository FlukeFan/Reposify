using System;
using System.Collections.Generic;
using System.Data.Entity;
using Reposify.Queries;

namespace Reposify.Ef6
{
    public class Ef6Repository<TId> : IIdentityMapRepository<TId>, IDisposable
    {
        private DbContext               _dbContext;
        private DbContextTransaction    _transaction;

        public Ef6Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual Ef6Repository<TId> Open()
        {
            _transaction = _dbContext.Database.BeginTransaction();
            return this;
        }

        public virtual void Commit()
        {
            _transaction.Commit();
        }

        public void Execute(IDbExecution dbExecution)
        {
        }

        public T Execute<T>(IDbQuery<T> dbQuery)
        {
            return default(T);
        }

        public virtual T Save<T>(T entity) where T : class, IEntity<TId>
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual T Load<T>(TId id) where T : class, IEntity<TId>
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity<TId>
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public virtual void Flush()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual Query<T, TId> Query<T>() where T : class, IEntity<TId>
        {
            return new Query<T, TId>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T, TId> query) where T : class, IEntity<TId>
        {
            var all = _dbContext.Set<T>().ToListAsync().GetAwaiter().GetResult();
            return all;
        }

        public virtual void Dispose()
        {
            try
            {
                using (_transaction)
                {
                    if (_transaction != null)
                        _transaction.Rollback();
                }
            }
            finally
            {
                try
                {
                    using (_dbContext) { }
                }
                finally
                {
                    _transaction = null;
                    _dbContext = null;
                }
            }
        }
    }
}

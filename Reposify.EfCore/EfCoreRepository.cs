﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Reposify.EfCore
{
    public class EfCoreRepository :
        IIdentityMapRepository,
        IRepositoryAsync,
        IDbExecutor,
        IDbExecutorAsync,
        ILinqQueryable,
        IDbLinqExecutor,
        IDisposable
    {
        protected DbContext                 _dbContext;
        protected IDbContextTransaction     _transaction;
        protected EfCoreHandlers            _handlers = new EfCoreHandlers();

        public DbContext                DbContext   { get { return _dbContext; } }
        public IDbContextTransaction    Transaction { get { return _transaction; } }

        public EfCoreRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual EfCoreRepository Open()
        {
            _transaction = _dbContext.Database.BeginTransaction();
            return this;
        }

        public virtual void Commit()
        {
            _transaction.Commit();
        }

        public EfCoreRepository UsingHandlers(EfCoreHandlers handlers)
        {
            _handlers = handlers;
            return this;
        }

        public void Execute(IDbExecution dbExecution)
        {
            _handlers.Execute(this, dbExecution);
        }

        public T Execute<T>(IDbQuery<T> dbQuery)
        {
            return _handlers.Execute(this, dbQuery);
        }

        public Task ExecuteAsync(IDbExecution dbExecution)
        {
            return _handlers.ExecuteAsync(this, dbExecution);
        }

        public Task<T> ExecuteAsync<T>(IDbQuery<T> dbQuery)
        {
            return _handlers.ExecuteAsync(this, dbQuery);
        }

        public virtual T Save<T>(T entity) where T : class, IEntity
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual T Load<T>(object id) where T : class, IEntity
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity
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

        public virtual async Task<T> SaveAsync<T>(T entity) where T : class, IEntity
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual Task<T> LoadAsync<T>(object id) where T : class, IEntity
        {
            return _dbContext.Set<T>().FindAsync(id);
        }

        public virtual Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            _dbContext.Set<T>().Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public TResult Execute<TEntity, TResult>(IDbLinq<TEntity, TResult> query) where TEntity : class
        {
            return query.Execute(Query<TEntity>());
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

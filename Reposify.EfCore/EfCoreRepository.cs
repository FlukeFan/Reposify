using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Reposify.EfCore
{
    public class EfCoreRepository :
        IRepository,
        IRepositoryAsync,
        IUnitOfWork,
        IUnitOfWorkAsync,
        IIdentityMapReloadable,
        IDbExecutor,
        IDbExecutorAsync,
        ILinqQueryable,
        IDbLinqExecutor,
        IDbLinqExecutorAsync,
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
            return entity;
        }

        public virtual T Load<T>(object id) where T : class, IEntity
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void Flush()
        {
            _dbContext.SaveChanges();
        }

        public virtual Task FlushAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public virtual void ReloadAll()
        {
            foreach (EntityEntry entry in _dbContext.ChangeTracker.Entries())
                entry.Reload();
        }

        public virtual async Task<T> SaveAsync<T>(T entity) where T : class, IEntity
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual Task<T> LoadAsync<T>(object id) where T : class, IEntity
        {
            return _dbContext.Set<T>().FindAsync(id);
        }

        public virtual Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public List<TEntity> List<TEntity>(IDbLinq<TEntity> query) where TEntity : class
        {
            return query.Prepare(Query<TEntity>()).ToList();
        }

        public long Count<TEntity>(IDbLinq<TEntity> query) where TEntity : class
        {
            return query.Prepare(Query<TEntity>()).Count();
        }

        public async Task<List<TEntity>> ListAsync<TEntity>(IDbLinq<TEntity> query) where TEntity : class
        {
            return await query.Prepare(Query<TEntity>()).ToListAsync();
        }

        public async Task<long> CountAsync<TEntity>(IDbLinq<TEntity> query) where TEntity : class
        {
            return await query.Prepare(Query<TEntity>()).CountAsync();
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

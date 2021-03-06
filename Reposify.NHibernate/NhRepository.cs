﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

namespace Reposify.NHibernate
{
    public class NhRepository :
        IRepository,
        IRepositoryAsync,
        IUnitOfWork,
        IUnitOfWorkAsync,
        IIdentityMapClearable,
        IDbExecutor,
        IDbExecutorAsync,
        ILinqQueryable,
        IDbLinqExecutor,
        IDbLinqExecutorAsync,
        IDisposable
    {
        /// <summary> creates a new session and begins a new transaction </summary>
        public static NhRepository Open(ISessionFactory sessionFactory, NhHandlers handlers = null)
        {
            var session = sessionFactory.OpenSession();
            var repository = new NhRepository(session);

            if (handlers != null)
                repository.UsingHandlers(handlers);

            return repository.BeginTransaction();
        }

        protected ISession          _session;
        protected ITransaction      _transaction;
        protected NhHandlers        _handlers       = new NhHandlers();

        public NhRepository(ISession session)
        {
            _session = session;
        }

        public ISession     Session     { get { return _session; } }
        public ITransaction Transaction { get { return _transaction; } }

        public virtual NhRepository BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
            return this;
        }

        public virtual void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public NhRepository UsingHandlers(NhHandlers handlers)
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
            _session.Save(entity);
            return entity;
        }

        public virtual T Load<T>(object id) where T : class, IEntity
        {
            return _session.Load<T>(id);
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity
        {
            _session.Delete(entity);
        }

        public virtual void Flush()
        {
            _session.Flush();
        }

        public virtual Task FlushAsync()
        {
            return _session.FlushAsync();
        }

        public virtual void Clear()
        {
            _session.Clear();
        }

        public virtual async Task<T> SaveAsync<T>(T entity) where T : class, IEntity
        {
            await _session.SaveAsync(entity);
            return entity;
        }

        public virtual async Task<T> LoadAsync<T>(object id) where T : class, IEntity
        {
            return await _session.LoadAsync<T>(id);
        }

        public async virtual Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            await _session.DeleteAsync(entity);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _session.Query<T>();
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
                using (_transaction) { }
            }
            finally
            {
                try
                {
                    using (_session) { }
                }
                finally
                {
                    _transaction = null;
                    _session = null;
                }
            }
        }
    }
}

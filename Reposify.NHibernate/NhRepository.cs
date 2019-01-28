﻿using System;
using System.Linq;
using NHibernate;

namespace Reposify.NHibernate
{
    public class NhRepository : IIdentityMapRepository, IDbExecutor, ILinqQueryable, IDbLinqExecutor, IDisposable
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

        public virtual void Clear()
        {
            _session.Clear();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _session.Query<T>();
        }

        public TResult Execute<TEntity, TResult>(IDbLinq<TEntity, TResult> query) where TEntity : class
        {
            return query.Execute(Query<TEntity>());
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

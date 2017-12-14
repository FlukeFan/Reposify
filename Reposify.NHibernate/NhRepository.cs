using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using Reposify.Queries;

namespace Reposify.NHibernate
{
    public class NhRepository<TId> : IIdentityMapRepository<TId>, IDisposable
    {
        public static ISessionFactory SessionFactory { get; protected set; }

        public static void Init(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        public static void Init(Configuration configuration)
        {
            Init(configuration.BuildSessionFactory());
        }

        protected ISession          _session;
        protected ITransaction      _transaction;
        protected NhHandlers<TId>   _handlers       = new NhHandlers<TId>();

        public ISession     Session     { get { return _session; } }
        public ITransaction Transaction { get { return _transaction; } }

        public virtual NhRepository<TId> Open()
        {
            if (SessionFactory == null)
                throw new Exception("Call Init() once to setup the session factory");

            _session = SessionFactory.OpenSession();
            _transaction = _session.BeginTransaction();
            return this;
        }

        public virtual void Commit()
        {
            _transaction.Commit();
            _transaction = null;
            _session = null;
        }

        public NhRepository<TId> UsingHandlers(NhHandlers<TId> handlers)
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

        public virtual T Save<T>(T entity) where T : class, IEntity<TId>
        {
            _session.Save(entity);
            return entity;
        }

        public virtual T Load<T>(TId id) where T : class, IEntity<TId>
        {
            return _session.Load<T>(id);
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity<TId>
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

        public virtual Query<T, TId> Query<T>() where T : class, IEntity<TId>
        {
            return new Query<T, TId>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T, TId> query) where T : class, IEntity<TId>
        {
            var nhCriteria = NhCriteria.For(query);
            var criteria = nhCriteria.CreateCriteria(_session);
            return criteria.List<T>();
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

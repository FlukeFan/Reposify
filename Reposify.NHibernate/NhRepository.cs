using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using Reposify.Queries;

namespace Reposify.NHibernate
{
    public class NhRepository : IIdentityMapRepository, IDisposable
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
        protected NhHandlers        _handlers       = new NhHandlers();

        public ISession     Session     { get { return _session; } }
        public ITransaction Transaction { get { return _transaction; } }

        public virtual NhRepository Open()
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

        public virtual Query<T> Query<T>() where T : class, IEntity
        {
            return new Query<T>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T> query) where T : class, IEntity
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

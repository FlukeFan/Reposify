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

        private ISession        _session;
        private ITransaction    _transaction;

        public NhRepository()
        {
            if (SessionFactory == null)
                throw new Exception("Call Init() once to setup the session factory");
        }

        public ISession     Session     { get { return _session; } }
        public ITransaction Transaction { get { return _transaction; } }

        public virtual NhRepository<TId> Open()
        {
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

        public virtual T Save<T>(T entity) where T : IEntity<TId>
        {
            _session.Save(entity);
            return entity;
        }

        public virtual T Load<T>(TId id) where T : IEntity<TId>
        {
            return _session.Load<T>(id);
        }

        public virtual void Flush()
        {
            _session.Flush();
        }

        public virtual void Clear()
        {
            _session.Clear();
        }

        public virtual Query<T, TId> Query<T>() where T : IEntity<TId>
        {
            return new Query<T, TId>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T, TId> query) where T : IEntity<TId>
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

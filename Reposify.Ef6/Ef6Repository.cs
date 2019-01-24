﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Reposify.Queries;

namespace Reposify.Ef6
{
    public class Ef6Repository : IIdentityMapRepository, IDisposable
    {
        protected DbContext                 _dbContext;
        protected DbContextTransaction      _transaction;
        protected Ef6Handlers               _handlers = new Ef6Handlers();

        public DbContext            DbContext   { get { return _dbContext; } }
        public DbContextTransaction Transaction { get { return _transaction; } }

        public Ef6Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual Ef6Repository Open()
        {
            _transaction = _dbContext.Database.BeginTransaction();
            return this;
        }

        public virtual void Commit()
        {
            _transaction.Commit();
        }

        public Ef6Repository UsingHandlers(Ef6Handlers handlers)
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

        public virtual Query<T> Query<T>() where T : class, IEntity
        {
            return new Query<T>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T> query) where T : class, IEntity
        {
            var dbSet = (IEnumerable<T>)_dbContext.Set<T>();

            foreach (var restriction in query.Restrictions)
            {
                var expression = Where.Lambda<T>(restriction).Compile();
                dbSet = dbSet.Where(expression);
            }

            foreach (var order in query.Orders)
            {
                var processor = Ordering.Lambda<T>(order).Compile();
                dbSet = processor(dbSet);
            }

            if (query.SkipCount.HasValue)
                dbSet = dbSet.Skip(query.SkipCount.Value);

            if (query.TakeCount.HasValue)
                dbSet = dbSet.Take(query.TakeCount.Value);

            return dbSet.ToList();
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
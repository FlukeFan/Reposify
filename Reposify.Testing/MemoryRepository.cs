using System;
using System.Collections.Generic;
using System.Linq;

namespace Reposify.Testing
{
    public class MemoryRepository : IRepository, IDbExecutor, IDbLinqExecutor, ILinqQueryable, IDisposable
    {
        protected IDictionary<Type, Action<object>>         _executionHandlers  = new Dictionary<Type, Action<object>>();
        protected IDictionary<Type, Func<object, object>>   _queryHandlers      = new Dictionary<Type, Func<object, object>>();

        protected ConstraintChecker                         _constraintChecker;
        protected IList<IEntity>                            _entities           = new List<IEntity>();

        protected int lastId = 101;

        public MemoryRepository(ConstraintChecker constraintChecker)
        {
            _constraintChecker = constraintChecker;
        }

        public virtual void SetHandler<T>(Action<T> handler) where T : IDbExecution
        {
            _executionHandlers[typeof(T)] = q => handler((T)q);
        }

        public virtual void SetHandler<TQuery, TResult>(Func<TQuery, TResult> handler) where TQuery : IDbQuery<TResult>
        {
            _queryHandlers[typeof(TQuery)] = q => handler((TQuery)q);
        }

        public virtual void Execute(IDbExecution dbExecution)
        {
            if (dbExecution == null)
                throw new Exception("attempt to execute null query");

            var type = dbExecution.GetType();

            if (!_executionHandlers.ContainsKey(type))
                throw new Exception($"no handler has been set for {type} - use SetHandler<T>() to set a handler");

            _executionHandlers[type](dbExecution);
        }

        public virtual T Execute<T>(IDbQuery<T> dbQuery)
        {
            if (dbQuery == null)
                throw new Exception("attempt to execute null query");

            var type = dbQuery.GetType();

            if (!_queryHandlers.ContainsKey(type))
                throw new Exception($"no handler has been set for {type} - use SetHandler<T>() to set a handler");

            return (T)_queryHandlers[type](dbQuery);
        }

        public virtual T Save<T>(T entity) where T : class, IEntity
        {
            if (entity == null)
                throw new Exception("Entity to be saved should not be null");

            _constraintChecker.BeforeSave(entity);
            CustomChecks.Check(entity, _constraintChecker);
            var idProperty = entity.GetType().GetProperty("Id");
            idProperty.SetValue(entity, lastId++);
            _entities.Add(entity);
            return entity;
        }

        public virtual T Load<T>(object id) where T : class, IEntity
        {
            return _entities
                .Where(e => e.Id.Equals(id))
                .Cast<T>()
                .SingleOrDefault();
        }

        public virtual void Delete<T>(T entity) where T : class, IEntity
        {
            _entities.Remove(entity);
        }

        public virtual void Flush()
        {
            // no externally visible behaviour to implement
        }

        public IList<T> All<T>() where T : class, IEntity
        {
            ILinqQueryable qr = this;
            return qr.Query<T>().ToList();
        }

        public void ShouldContain(IEntity entity)
        {
            if (entity == null)
                throw new Exception("Entity to be verified should not be null");

            if (entity.Id == null || entity.Id.Equals(Activator.CreateInstance(entity.Id.GetType())))
                throw new Exception("Entity to be verified has an unsaved Id value: " + entity.Id);

            if (!_entities.Contains(entity))
                throw new Exception(string.Format("Could not find Entity with Id {0} in Repository", entity.Id));
        }

        public void ShouldContain<T>(object id)
        {
            var entity = _entities.Where(e => e.Id.Equals(id)).SingleOrDefault();

            if (entity == null)
                throw new Exception(string.Format("Could not find entity with id {0} and type {1} in the Repository", id, typeof(T)));
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _entities
                .Where(e => typeof(T).IsAssignableFrom(e.GetType()))
                .Cast<T>()
                .AsQueryable();
        }

        public TResult Execute<TEntity, TResult>(IDbLinq<TEntity, TResult> query) where TEntity : class
        {
            return query.Execute(Query<TEntity>());
        }

        public void Dispose()
        {
        }
    }
}

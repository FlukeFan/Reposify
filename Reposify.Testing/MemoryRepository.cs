using System;
using System.Collections.Generic;
using System.Linq;
using Reposify.Queries;

namespace Reposify.Testing
{
    public class MemoryRepository : IRepository, IDisposable
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
            return Query<T>().List();
        }

        public virtual Query<T> Query<T>() where T : class, IEntity
        {
            return new Query<T>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T> query) where T : class, IEntity
        {
            var entities = _entities.Where(e => typeof(T).IsAssignableFrom(e.GetType())).Cast<T>();

            foreach (var restriction in query.Restrictions)
                entities = Filter(entities, restriction);

            foreach (var order in query.Orders)
                entities = OrderBy(entities, order);

            if (query.SkipCount.HasValue)
                entities = entities.Skip(query.SkipCount.Value);

            if (query.TakeCount.HasValue)
                entities = entities.Take(query.TakeCount.Value);

            return entities.ToList();
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

        private static IEnumerable<T> Filter<T>(IEnumerable<T> entities, Where restriction)
        {
            var expression = Where.Lambda<T>(restriction).Compile();
            entities = entities.Where(expression);
            return entities;
        }

        private static IEnumerable<T> OrderBy<T>(IEnumerable<T> entities, Ordering order)
        {
            var processor = Ordering.Lambda<T>(order).Compile();
            entities = processor(entities);
            return entities;
        }

        public void Dispose()
        {
        }
    }
}

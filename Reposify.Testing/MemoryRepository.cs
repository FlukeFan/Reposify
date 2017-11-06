using System;
using System.Collections.Generic;
using System.Linq;
using Reposify.Queries;

namespace Reposify.Testing
{
    public class MemoryRepository<TId> : IRepository<TId>
    {
        private ConstraintChecker       _constraintChecker;
        private IList<IEntity<TId>>     _entities = new List<IEntity<TId>>();

        private int lastId = 101;

        public MemoryRepository(ConstraintChecker constraintChecker)
        {
            _constraintChecker = constraintChecker;
        }

        public virtual T Save<T>(T entity) where T : IEntity<TId>
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

        public virtual T Load<T>(TId id) where T : IEntity<TId>
        {
            return _entities
                .Where(e => e.Id.Equals(id))
                .Cast<T>()
                .SingleOrDefault();
        }

        public virtual void Delete<T>(T entity) where T : IEntity<TId>
        {
            _entities.Remove(entity);
        }

        public virtual void Flush()
        {
            // no externally visible behaviour to implement
        }

        public IList<T> All<T>() where T : IEntity<TId>
        {
            return Query<T>().List();
        }

        public virtual Query<T, TId> Query<T>() where T : IEntity<TId>
        {
            return new Query<T, TId>(this);
        }

        public virtual IList<T> Satisfy<T>(Query<T, TId> query) where T : IEntity<TId>
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

        public void ShouldContain(IEntity<TId> entity)
        {
            if (entity == null)
                throw new Exception("Entity to be verified should not be null");

            if (entity.Id == null || entity.Id.Equals(default(TId)))
                throw new Exception("Entity to be verified has an unsaved Id value: " + entity.Id);

            if (!_entities.Contains(entity))
                throw new Exception(string.Format("Could not find Entity with Id {0} in Repository", entity.Id));
        }

        public void ShouldContain<T>(TId id)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reposify.Testing
{
    public class MemoryRepository :
        IRepository,
        IRepositoryAsync,
        IUnitOfWork,
        IUnitOfWorkAsync,
        IDbExecutor,
        IDbExecutorAsync,
        IDbLinqExecutor,
        ILinqQueryable,
        IDisposable
    {
        protected MemoryHandlers    _handlers           = new MemoryHandlers();
        protected ConstraintChecker _constraintChecker;
        protected IList<IEntity>    _entities           = new List<IEntity>();

        protected int lastId = 101;

        public MemoryRepository(ConstraintChecker constraintChecker)
        {
            _constraintChecker = constraintChecker;
        }

        public MemoryRepository UsingHandlers(MemoryHandlers handlers)
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

        public virtual Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task<T> SaveAsync<T>(T entity) where T : class, IEntity
        {
            return Task.FromResult(Save(entity));
        }

        public virtual Task<T> LoadAsync<T>(object id) where T : class, IEntity
        {
            return Task.FromResult(Load<T>(id));
        }

        public virtual Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            Delete(entity);
            return Task.CompletedTask;
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

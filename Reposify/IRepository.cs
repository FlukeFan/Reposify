namespace Reposify
{
    public interface IRepository
    {
        T       Save<T>(T entity)   where T : class, IEntity;
        T       Load<T>(object id)  where T : class, IEntity;
        void    Delete<T>(T entity) where T : class, IEntity;
        void    Flush(); // TODO: move to IUnitOfWork
    }
}

namespace Reposify
{
    public interface IRepository
    {
        void            Execute(IDbExecution dbExecution);
        T               Execute<T>(IDbQuery<T> dbQuery);

        T               Save<T>(T entity)                   where T : class, IEntity;
        T               Load<T>(object id)                  where T : class, IEntity;
        void            Delete<T>(T entity)                 where T : class, IEntity;
        void            Flush();
    }
}

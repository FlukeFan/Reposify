namespace Reposify
{
    public interface IDbExecutor
    {
        void    Execute(IDbExecution dbExecution);
        T       Execute<T>(IDbQuery<T> dbQuery);
    }
}

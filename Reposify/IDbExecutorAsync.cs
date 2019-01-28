using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbExecutorAsync
    {
        Task    Execute(IDbExecution dbExecution);
        Task<T> Execute<T>(IDbQuery<T> dbQuery);
    }
}

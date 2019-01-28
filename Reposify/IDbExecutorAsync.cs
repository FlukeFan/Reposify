using System.Threading.Tasks;

namespace Reposify
{
    public interface IDbExecutorAsync
    {
        Task    ExecuteAsync(IDbExecution dbExecution);
        Task<T> ExecuteAsync<T>(IDbQuery<T> dbQuery);
    }
}

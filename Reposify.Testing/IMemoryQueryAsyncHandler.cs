using System.Threading.Tasks;

namespace Reposify.Testing
{
    public interface IMemoryQueryAsyncHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        Task<TResult> ExecuteAsync(MemoryRepository repository, TDbQuery dbquery);
    }
}

using System.Threading.Tasks;

namespace Reposify.EfCore
{
    public interface IEfCoreQueryAsyncHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        Task<TResult> ExecuteAsync(EfCoreRepository repository, TDbQuery dbquery);
    }
}

using System.Threading.Tasks;

namespace Reposify.NHibernate
{
    public interface INhQueryAsyncHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        Task<TResult> ExecuteAsync(NhRepository repository, TDbQuery dbquery);
    }
}

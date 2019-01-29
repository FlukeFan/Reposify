using System.Threading.Tasks;

namespace Reposify.NHibernate
{
    public interface INhExecutionAsyncHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        Task ExecuteAsync(NhRepository repository, TDbExecution dbExecution);
    }
}

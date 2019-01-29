using System.Threading.Tasks;

namespace Reposify.EfCore
{
    public interface IEfCoreExecutionAsyncHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        Task ExecuteAsync(EfCoreRepository repository, TDbExecution dbExecution);
    }
}

using System.Threading.Tasks;

namespace Reposify.Testing
{
    public interface IMemoryExecutionAsyncHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        Task ExecuteAsync(MemoryRepository repository, TDbExecution dbExecution);
    }
}

namespace Reposify.Testing
{
    public interface IMemoryExecutionHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(MemoryRepository repository, TDbExecution dbExecution);
    }
}

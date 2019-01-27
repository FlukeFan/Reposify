namespace Reposify.EfCore
{
    public interface IEfCoreExecutionHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(EfCoreRepository repository, TDbExecution dbExecution);
    }
}

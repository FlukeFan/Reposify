namespace Reposify.Ef6
{
    public interface IEf6ExecutionHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(Ef6Repository repository, TDbExecution dbExecution);
    }
}

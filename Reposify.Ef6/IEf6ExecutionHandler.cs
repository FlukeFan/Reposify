namespace Reposify.Ef6
{
    public interface IEf6ExecutionHandler<TId, TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(Ef6Repository<TId> repository, TDbExecution dbExecution);
    }
}

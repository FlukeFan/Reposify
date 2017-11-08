namespace Reposify.NHibernate
{
    public interface INhExecutionHandler<TId, TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(NhRepository<TId> repository, TDbExecution dbExecution);
    }
}

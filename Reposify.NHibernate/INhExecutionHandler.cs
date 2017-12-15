namespace Reposify.NHibernate
{
    public interface INhExecutionHandler<TDbExecution> where TDbExecution : IDbExecution
    {
        void Execute(NhRepository repository, TDbExecution dbExecution);
    }
}

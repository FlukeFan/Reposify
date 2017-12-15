namespace Reposify.NHibernate
{
    public interface INhQueryHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(NhRepository repository, TDbQuery dbquery);
    }
}

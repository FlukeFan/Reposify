namespace Reposify.NHibernate
{
    public interface INhQueryHandler<TId, TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(NhRepository<TId> repository, TDbQuery dbquery);
    }
}

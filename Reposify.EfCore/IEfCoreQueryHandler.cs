namespace Reposify.EfCore
{
    public interface IEfCoreQueryHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(EfCoreRepository repository, TDbQuery dbquery);
    }
}

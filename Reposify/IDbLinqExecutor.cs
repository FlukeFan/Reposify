namespace Reposify
{
    public interface IDbLinqExecutor
    {
        TResult Execute<TEntity, TResult>(IDbLinq<TEntity, TResult> query) where TEntity : class;
    }
}

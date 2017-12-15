namespace Reposify.Ef6
{
    public interface IEf6QueryHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(Ef6Repository repository, TDbQuery dbquery);
    }
}

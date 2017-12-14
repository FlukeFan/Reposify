namespace Reposify.Ef6
{
    public interface IEf6QueryHandler<TId, TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(Ef6Repository<TId> repository, TDbQuery dbquery);
    }
}

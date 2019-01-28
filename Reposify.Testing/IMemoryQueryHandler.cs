namespace Reposify.Testing
{
    public interface IMemoryQueryHandler<TDbQuery, TResult> where TDbQuery : IDbQuery<TResult>
    {
        TResult Execute(MemoryRepository repository, TDbQuery dbquery);
    }
}

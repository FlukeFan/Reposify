namespace Reposify
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}

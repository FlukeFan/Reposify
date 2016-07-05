namespace Reposify
{
    public interface IIdentityMapRepository<TId> : IRepository<TId>
    {
        void Clear();
    }
}

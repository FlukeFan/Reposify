namespace Reposify
{
    public interface IIdentityMapRepository : IRepository // TODO: make this a separate interface without inheritance
    {
        void Clear();
    }
}

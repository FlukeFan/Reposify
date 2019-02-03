using System.Threading.Tasks;

namespace Reposify.Testing
{
    public static class BuilderExtensions
    {
        public static T Save<T>(this Builder<T> builder, IRepository repository) where T : class, IEntity
        {
            var entity = builder.Value();
            return repository.Save(entity);
        }

        public static Task<T> SaveAsync<T>(this Builder<T> builder, IRepositoryAsync repository) where T : class, IEntity
        {
            var entity = builder.Value();
            return repository.SaveAsync(entity);
        }
    }
}

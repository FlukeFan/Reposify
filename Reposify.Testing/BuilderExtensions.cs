using System.Threading.Tasks;

namespace Reposify.Testing
{
    public static class BuilderExtensions
    {
        public static T Save<T>(this Builder<T> builder, IRepository repository) where T : class, IEntity
        {
            var entity = builder.Value();
            entity = repository.Save(entity);
            (repository as IUnitOfWork)?.Flush();
            return entity;
        }

        public static async Task<T> SaveAsync<T>(this Builder<T> builder, IRepositoryAsync repository) where T : class, IEntity
        {
            var entity = builder.Value();
            entity = await repository.SaveAsync(entity);
            await (repository as IUnitOfWorkAsync)?.FlushAsync();
            return entity;
        }
    }
}

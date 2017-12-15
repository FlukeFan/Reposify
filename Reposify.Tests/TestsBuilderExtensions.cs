using Reposify.Testing;

namespace Reposify.Tests
{
    public static class TestsBuilderExtensions
    {
        public static T Save<T>(this Builder<T> builder, IRepository repository) where T : class, IEntity
        {
            var entity = builder.Value();
            return repository.Save(entity);
        }
    }
}

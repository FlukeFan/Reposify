using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IDbExecutorAsyncTests : IDisposableTests
    {
        private IRepository         Repository      { get => (IRepository)_disposable; }
        private IDbExecutorAsync    DbExecutorAsync { get => (IDbExecutorAsync)_disposable; }

        [Test]
        public async virtual Task DbQuery_IsImplemented()
        {
            var poly1 = new PolyTypeBuilder().With(p => p.String, "poly1").Value();
            var poly2 = new PolyTypeBuilder().With(p => p.String, "poly2").Value();
            var poly3 = new PolyTypeBuilder().With(p => p.String, "poly3").Value();

            var save = new QuerySaveEntities { EntitiesToSave = new[] { poly1, poly2, poly3 } };
            await DbExecutorAsync.ExecuteAsync(save);

            var query = new QueryIn { IntValues = new[] { poly1.Id, poly3.Id } };
            var entitiesWithId = await DbExecutorAsync.ExecuteAsync(query);

            entitiesWithId.Select(e => e.String).Should().BeEquivalentTo("poly1", "poly3");
        }
    }
}

using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IDbExecutorTests : IDisposableTests
    {
        private IRepository     Repository  { get => (IRepository)_disposable; }
        private IDbExecutor     DbExecutor  { get => (IDbExecutor)_disposable; }

        [Test]
        public virtual void DbQuery_IsImplemented()
        {
            var poly1 = new PolyTypeBuilder().With(p => p.String, "poly1").Value();
            var poly2 = new PolyTypeBuilder().With(p => p.String, "poly2").Value();
            var poly3 = new PolyTypeBuilder().With(p => p.String, "poly3").Value();

            var save = new QuerySaveEntities { EntitiesToSave = new[] { poly1, poly2, poly3 } };
            DbExecutor.Execute(save);

            var query = new QueryIn { IntValues = new[] { poly1.Id, poly3.Id } };
            var entitiesWithId = DbExecutor.Execute(query);

            entitiesWithId.Select(e => e.String).Should().BeEquivalentTo("poly1", "poly3");
        }
    }
}

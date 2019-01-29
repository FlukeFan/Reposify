using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IDbLinqExecutorAsyncTests : IDisposableTests
    {
        private IRepository             Repository          { get => (IRepository)_disposable; }
        private IDbLinqExecutorAsync    DbLinqExecutorAsync { get => (IDbLinqExecutorAsync)_disposable; }

        [Test]
        public async virtual Task Execute()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            var count = await DbLinqExecutorAsync.ExecuteAsync(new QueryUsingLinq { StringValue = "poly2" });

            count.Should().Be(2);
        }
    }
}

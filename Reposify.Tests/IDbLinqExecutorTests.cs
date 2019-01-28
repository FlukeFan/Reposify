using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IDbLinqExecutorTests : IDisposableTests
    {
        private IRepository     Repository      { get => (IRepository)_disposable; }
        private IDbLinqExecutor DbLinqExecutor  { get => (IDbLinqExecutor)_disposable; }

        [Test]
        public virtual void Execute()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            var count = DbLinqExecutor.Execute(new QueryUsingLinq { StringValue = "poly2" });

            count.Should().Be(2);
        }
    }
}

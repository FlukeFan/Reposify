using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IDbLinqExecutorAsyncTests : IDisposableTests
    {
        private IRepository             Repository      { get => (IRepository)_disposable; }
        private IUnitOfWork             UnitOfWork      { get => (IUnitOfWork)_disposable; }
        private IDbLinqExecutorAsync    DbLinqExecutor  { get => (IDbLinqExecutorAsync)_disposable; }

        [Test]
        public virtual async Task Execute()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            UnitOfWork.Flush();

            var list = await DbLinqExecutor.ListAsync(new QueryUsingLinq { StringValue = "poly2" });

            list.Count.Should().Be(2);
        }
    }
}

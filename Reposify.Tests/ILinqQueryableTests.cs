using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class ILinqQueryableTests : IDisposableTests
    {
        private IRepository     Repository      { get => (IRepository)_disposable; }
        private IUnitOfWork     UnitOfWork      { get => (IUnitOfWork)_disposable; }
        private ILinqQueryable  LinqQueryable   { get => (ILinqQueryable)_disposable; }

        [Test]
        public virtual void Query()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            UnitOfWork.Flush();

            var list = LinqQueryable.Query<PolyType>()
                .Where(pt => pt.String == "poly2")
                .ToList();

            list.Count.Should().Be(2);
        }
    }
}

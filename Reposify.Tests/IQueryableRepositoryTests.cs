using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IQueryableRepositoryTests : IDisposableTests
    {
        private IRepository             Repository  { get => (IRepository)_disposable; }
        private IQueryableRepository    Queryable   { get => (IQueryableRepository)_disposable; }

        [Test]
        public virtual void QueryThroughQueryObject()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            var count = Queryable.Execute(new QueryUsingLinq { StringValue = "poly2" });

            count.Should().Be(2);
        }

        [Test]
        public virtual void QueryDirect()
        {
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly1").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly2").Value());
            Repository.Save(new PolyTypeBuilder().With(p => p.String, "poly3").Value());

            var list = Queryable.Query<PolyType>()
                .Where(pt => pt.String == "poly2")
                .ToList();

            list.Count.Should().Be(2);
        }
    }
}

using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests
{
    public abstract class IRepositoryTests : IDisposableTests
    {
        private IRepository     Repository      { get => (IRepository)_disposable; }
        private ILinqQueryable  LinqQueryable   { get => (ILinqQueryable)_disposable; }

        [Test]
        public virtual void Save_SetsId()
        {
            var poly = new PolyTypeBuilder().Value();

            poly.Id.Should().Be(0, "newly instantiated entity should have a default Id value");

            var savedUser = Repository.Save(poly);

            savedUser.Should().BeSameAs(poly);
            savedUser.Id.Should().NotBe(0, "persisted entity should have a non-zero Id");
        }

        [Test]
        public virtual void Load_RetrievesSavedObject()
        {
            var poly = new PolyTypeBuilder().Value();

            Repository.Save(poly);
            poly.Id.Should().NotBe(0);

            var loaded = Repository.Load<PolyType>(poly.Id);
            loaded.Should().BeSameAs(poly);
        }

        [Test]
        public virtual void Load_IdentityMapEnsuresReferencesEqual()
        {
            var poly = new PolyTypeBuilder().Value();

            Repository.Save(poly);
            poly.Id.Should().NotBe(0);

            var loaded1 = Repository.Load<PolyType>(poly.Id);
            var loaded2 = Repository.Load<PolyType>(poly.Id);

            loaded1.Should().BeSameAs(loaded2);
        }

        [Test]
        public virtual void Delete_RemovesEntity()
        {
            var poly1 = new PolyTypeBuilder().Save(Repository);
            var poly2 = new PolyTypeBuilder().Save(Repository);

            Repository.Delete(poly1);

            var allSaved = LinqQueryable.Query<PolyType>().ToList();

            allSaved.Count.Should().Be(1, "poly2 should have been deleted from the repository");
            allSaved[0].Id.Should().NotBe(poly1.Id);
            allSaved[0].Id.Should().Be(poly2.Id);
        }

        [Test]
        public void CheckSaveLoad()
        {
            var subType = new PolyTypeBuilder().Save(Repository);

            var entity = new PolyTypeBuilder()
                .With(e => e.SubType, subType)
                .Value();

            entity.CheckSaveLoad(Repository).Check();
        }
    }
}

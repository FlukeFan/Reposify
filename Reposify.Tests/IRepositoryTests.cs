using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Queries;

namespace Reposify.Tests
{
    [TestFixture]
    public abstract class IRepositoryTests
    {
        abstract protected IRepository<int> New();

        private IRepository<int> _repository;

        [SetUp]
        public virtual void SetUp()
        {
            _repository = New();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _repository = null;
        }

        [Test]
        public virtual void Save_SetsId()
        {
            var poly = new PolyTypeBuilder().Value();

            poly.Id.Should().Be(0, "newly instantiated entity should have a default Id value");

            var savedUser = _repository.Save(poly);

            savedUser.Should().BeSameAs(poly);
            savedUser.Id.Should().NotBe(0, "persisted entity should have a non-zero Id");
        }

        [Test]
        public virtual void Load_RetrievesSavedObject()
        {
            var poly = new PolyTypeBuilder().Value();

            _repository.Save(poly);
            poly.Id.Should().NotBe(0);

            var loaded = _repository.Load<PolyType>(poly.Id);
            loaded.Should().BeSameAs(poly);
        }

        [Test]
        public virtual void Load_IdentityMapEnsuresReferencesEqual()
        {
            var poly = new PolyTypeBuilder().Value();

            _repository.Save(poly);
            poly.Id.Should().NotBe(0);

            var loaded1 = _repository.Load<PolyType>(poly.Id);
            var loaded2 = _repository.Load<PolyType>(poly.Id);

            loaded1.Should().BeSameAs(loaded2);
        }

        [Test]
        public virtual void Delete_RemovesEntity()
        {
            var poly1 = new PolyTypeBuilder().Save(_repository);
            var poly2 = new PolyTypeBuilder().Save(_repository);

            _repository.Delete(poly1);

            var allSaved = _repository.Query<PolyType>().List();

            allSaved.Count.Should().Be(1, "poly2 should have been deleted from the repository");
            allSaved[0].Id.Should().NotBe(poly1.Id);
            allSaved[0].Id.Should().Be(poly2.Id);
        }

        [Test]
        public virtual void Query_RetrieveAll()
        {
            var poly1 = new PolyTypeBuilder().Save(_repository);
            var poly2 = new PolyTypeBuilder().Save(_repository);

            var allUsers = _repository.Query<PolyType>().List();

            allUsers.Count.Should().Be(2);
            allUsers.Should().Contain(poly1);
            allUsers.Should().Contain(poly2);
        }

        [Test]
        public virtual void Query_RestrictStringPropertyEqual()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.String, "test1@user.net").Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.String, "test2@user.net").Save(_repository);

            var specificUser =
                _repository.Query<PolyType>()
                    .Filter(e => e.String == "test2@user.net")
                    .SingleOrDefault();

            specificUser.Should().NotBeNull();
            specificUser.Should().BeSameAs(poly2);
        }

        [Test]
        public virtual void Query_RestrictComparisons()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(_repository);

            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int < 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int <= 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id, poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int == 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int > 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly3.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int >= 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly3.Id, poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Int != 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictBooleans()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Boolean, true).Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Boolean, false).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Boolean, true).Save(_repository);

            {
                var result = _repository.Query<PolyType>().Filter(e => e.Boolean == true).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Boolean == false).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Boolean).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => !e.Boolean).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Boolean != false).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Boolean != true).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictEnums()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val1).Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val2).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val3).Save(_repository);

            {
                var result = _repository.Query<PolyType>().Filter(e => e.Enum == PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Enum != PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Enum > PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.Enum >= PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictNull()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, 5   ).Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.NullableString, null      ).With(u => u.NullableInt, 5   ).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, 5   ).Save(_repository);
            var poly4 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, null).Save(_repository);

            {
                var result = _repository.Query<PolyType>().Filter(e => e.NullableString == null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.NullableString != null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id, poly4.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.NullableInt == null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly4.Id);
            }
            {
                var result = _repository.Query<PolyType>().Filter(e => e.NullableInt != null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly2.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_Ordering()
        {
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(_repository);
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(_repository);

            {
                var result = _repository.Query<PolyType>().OrderBy(e => e.Int, Direction.Ascending).List();
                result.Select(e => e.Int).Should().BeInAscendingOrder(e => e);
            }
            {
                var result = _repository.Query<PolyType>().OrderBy(e => e.Int, Direction.Descending).List();
                result.Select(e => e.Int).Should().BeInDescendingOrder(e => e);
            }
        }

        [Test]
        public virtual void Query_Paging()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(_repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(_repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(_repository);
            var poly4 = new PolyTypeBuilder().With(u => u.Int, 4).Save(_repository);

            var result = _repository.Query<PolyType>()
                .OrderBy(e => e.Int, Direction.Ascending)
                .Skip(1)
                .Take(2)
                .List();

            result.Select(e => e.Int).Should().BeEquivalentTo(2, 3);
        }
    }
}

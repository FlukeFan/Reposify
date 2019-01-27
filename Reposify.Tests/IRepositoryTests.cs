using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IRepositoryTests : IDisposableTests
    {
        private IRepository Repository { get => (IRepository)_disposable; }

        [Test]
        public virtual void DbQuery_IsImplemented()
        {
            var poly1 = new PolyTypeBuilder().With(p => p.String, "poly1").Value();
            var poly2 = new PolyTypeBuilder().With(p => p.String, "poly2").Value();
            var poly3 = new PolyTypeBuilder().With(p => p.String, "poly3").Value();

            var save = new QuerySaveEntities { EntitiesToSave = new[] { poly1, poly2, poly3 } };
            Repository.Execute(save);

            var query = new QueryIn { IntValues = new[] { poly1.Id, poly3.Id } };
            var entitiesWithId = Repository.Execute(query);

            entitiesWithId.Select(e => e.String).Should().BeEquivalentTo("poly1", "poly3");
        }

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

            var allSaved = Repository.Query<PolyType>().List();

            allSaved.Count.Should().Be(1, "poly2 should have been deleted from the repository");
            allSaved[0].Id.Should().NotBe(poly1.Id);
            allSaved[0].Id.Should().Be(poly2.Id);
        }

        [Test]
        public virtual void Query_RetrieveAll()
        {
            var poly1 = new PolyTypeBuilder().Save(Repository);
            var poly2 = new PolyTypeBuilder().Save(Repository);

            var allUsers = Repository.Query<PolyType>().List();

            allUsers.Count.Should().Be(2);
            allUsers.Should().Contain(poly1);
            allUsers.Should().Contain(poly2);
        }

        [Test]
        public virtual void Query_RestrictStringPropertyEqual()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.String, "test1@user.net").Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.String, "test2@user.net").Save(Repository);

            var specificUser =
                Repository.Query<PolyType>()
                    .Filter(e => e.String == "test2@user.net")
                    .SingleOrDefault();

            specificUser.Should().NotBeNull();
            specificUser.Should().BeSameAs(poly2);
        }

        [Test]
        public virtual void Query_RestrictComparisons()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(Repository);

            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int < 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int <= 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id, poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int == 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int > 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly3.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int >= 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly3.Id, poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Int != 2).List();
                result.Select(p => p.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictBooleans()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Boolean, true).Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Boolean, false).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Boolean, true).Save(Repository);

            {
                var result = Repository.Query<PolyType>().Filter(e => e.Boolean == true).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Boolean == false).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Boolean).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => !e.Boolean).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Boolean != false).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id, poly1.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Boolean != true).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictEnums()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val1).Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val2).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Enum, PolyType.Values.Val3).Save(Repository);

            {
                var result = Repository.Query<PolyType>().Filter(e => e.Enum == PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Enum != PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Enum > PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly3.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.Enum >= PolyType.Values.Val2).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_RestrictNull()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, 5   ).Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.NullableString, null      ).With(u => u.NullableInt, 5   ).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, 5   ).Save(Repository);
            var poly4 = new PolyTypeBuilder().With(u => u.NullableString, "not null").With(u => u.NullableInt, null).Save(Repository);

            {
                var result = Repository.Query<PolyType>().Filter(e => e.NullableString == null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly2.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.NullableString != null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly3.Id, poly4.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.NullableInt == null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly4.Id);
            }
            {
                var result = Repository.Query<PolyType>().Filter(e => e.NullableInt != null).List();
                result.Select(e => e.Id).Should().BeEquivalentTo(poly1.Id, poly2.Id, poly3.Id);
            }
        }

        [Test]
        public virtual void Query_Ordering()
        {
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(Repository);
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(Repository);

            {
                var result = Repository.Query<PolyType>().OrderBy(e => e.Int).List();
                result.Select(e => e.Int).Should().BeInAscendingOrder(e => e);
            }
            {
                var result = Repository.Query<PolyType>().OrderByDescending(e => e.Int).List();
                result.Select(e => e.Int).Should().BeInDescendingOrder(e => e);
            }
        }

        [Test]
        public virtual void Query_Paging()
        {
            var poly1 = new PolyTypeBuilder().With(u => u.Int, 1).Save(Repository);
            var poly2 = new PolyTypeBuilder().With(u => u.Int, 2).Save(Repository);
            var poly3 = new PolyTypeBuilder().With(u => u.Int, 3).Save(Repository);
            var poly4 = new PolyTypeBuilder().With(u => u.Int, 4).Save(Repository);

            var result = Repository.Query<PolyType>()
                .OrderBy(e => e.Int)
                .Skip(1)
                .Take(2)
                .List();

            result.Select(e => e.Int).Should().BeEquivalentTo(2, 3);
        }
    }
}

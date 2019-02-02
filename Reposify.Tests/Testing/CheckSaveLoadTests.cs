using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class CheckSaveLoadTests
    {
        [Test]
        public void WhenLoadedObjectMatches_Passes()
        {
            var entity = new PolyTypeBuilder(true).Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            var repository = new FakeMemoryRepository(loadedEntity);

            repository.CheckSaveLoad(entity).Check();
        }

        [Test]
        public void WhenInputPropertyIsNull_Fails()
        {
            var entity = new PolyTypeBuilder(true).With(pt => pt.BigString, null).Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("set to a not-null value");
        }

        [Test]
        public void WhenExcluded_DoesNotFail()
        {
            var entity = new PolyTypeBuilder(true).With(pt => pt.BigString, null).Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("set to a not-null value");

            var repository = new FakeMemoryRepository(loadedEntity);

            entity.CheckSaveLoad(repository)
                .ExcludeProperties("BigString")
                .Check();
        }

        [Test]
        public void WhenInputPropertyIsEmptyEnumerable_Fails()
        {
            var entity = new PolyTypeBuilder(true).With(pt => pt.BigString, "").Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("should not be empty");
        }

        [Test]
        public void WhenInputPropertyIsDefault_Fails()
        {
            var entity = new PolyTypeBuilder(true).With(pt => pt.Int, 0).Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("set to a not-default");
        }

        [Test]
        public void WhenOutputValuePropertyIsDifferent_Fails()
        {
            var entity = new PolyTypeBuilder(true).Value();
            var loadedEntity = new PolyTypeBuilder(true).With(le => le.Int, entity.Int + 10).Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("does not match");
        }

        [Test]
        public void WhenOutputStringPropertyIsDifferent_Fails()
        {
            var entity = new PolyTypeBuilder(true).With(e => e.BigString, "123").Value();
            var loadedEntity = new PolyTypeBuilder(true).With(le => le.BigString, "234").Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("does not match");
        }

        [Test]
        public void WhenOutputEnumerableIsDifferentLength_Fails()
        {
            var entity = new PolyTypeBuilder(true).With(e => e.BigString, "123").Value();
            var loadedEntity = new PolyTypeBuilder(true).With(le => le.BigString, "1234").Value();

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("does not match length");
        }

        [Test]
        public void WhenCustomCheck_CanFail()
        {
            var notAllowedValue = "this value is not allowed";

            CheckSaveLoad.CustomCheck = (prop, original, compare, name) =>
            {
                var originalValue = prop.GetValue(original, null);
                originalValue.Should().NotBe(notAllowedValue);
            };

            var entity = new PolyTypeBuilder(true).With(e => e.String, notAllowedValue).Value();
            var loadedEntity = new PolyTypeBuilder(true).With(le => le.String, notAllowedValue).Value();

            var repository = new FakeMemoryRepository(loadedEntity);

            Assert.That(() => entity.CheckSaveLoad(repository).Check(), Throws.Exception);
        }

        [Test]
        public void WhenEntityPropertyDiffers_Fails()
        {
            var entity = new PolyTypeBuilder(true).Value();
            var loadedEntity = new PolyTypeBuilder(true).Value();

            Builder.Modify(entity.SubType).With(st => st.Id, 123);
            Builder.Modify(loadedEntity.SubType).With(st => st.Id, 234);

            VerifyThrows(entity, loadedEntity).Message.Should().Contain("with Id");
        }

        private CheckSaveLoad.SaveLoadException VerifyThrows(PolyType entity, PolyType loadedEntity)
        {
            var repository = new FakeMemoryRepository(loadedEntity);
            var e = Assert.Throws<CheckSaveLoad.SaveLoadException>(() => entity.CheckSaveLoad(repository).Check());
            return e;
        }

        private class FakeMemoryRepository : MemoryRepository
        {
            private PolyType _entity;

            public FakeMemoryRepository(PolyType entity) : base(new ConstraintChecker())
            {
                _entity = entity;
            }

            public override T Load<T>(object id)
            {
                Builder.Modify(_entity).With(e => e.Id, id);
                return (T)(object)_entity;
            }
        }
    }
}

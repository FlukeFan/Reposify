using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryTests : IRepositoryTests
    {
        private MemoryRepository Repository { get => (MemoryRepository)_disposable; }

        protected override IDisposable New()
        {
            return new MemoryRepository(new ConstraintChecker());
        }

        [Test]
        public virtual void Flush_DoesNotThrow()
        {
            Repository.Flush();
        }

        [Test]
        public override void DbQuery_IsImplemented()
        {
            Repository.SetHandler<QuerySaveEntities>(q =>
            {
                foreach (var e in q.EntitiesToSave)
                    Repository.Save(e);
            });

            Repository.SetHandler<QueryIn, IList<PolyType>>(q =>
                Repository.Query<PolyType>().List()
                    .Where(p => q.IntValues.Contains(p.Id))
                    .ToList());

            base.DbQuery_IsImplemented();
        }

        [Test]
        public void ShouldContain()
        {
            var repository = new MemoryRepository(new ConstraintChecker());
            var entity = new PolyTypeBuilder().Value();

            repository.Save(entity);

            repository.ShouldContain(entity);
            repository.ShouldContain<PolyType>(entity.Id);
        }

        [Test]
        public void All_IsShortcutForQueryList()
        {
            var repository = new MemoryRepository(new ConstraintChecker());

            repository.Save(new PolyTypeBuilder().Value());
            repository.Save(new PolyTypeBuilder().Value());
            repository.Save(new PolyTypeBuilder().Value());

            repository.All<PolyType>().Should().HaveCount(3);
        }

        [Test]
        public void ShouldContain_Throws()
        {
            var repository = new MemoryRepository(new ConstraintChecker());

            Assert.Throws<Exception>(() => repository.ShouldContain(null)).Message.Should().Contain("should not be null");
            Assert.Throws<Exception>(() => repository.ShouldContain(new PolyTypeBuilder().Value())).Message.Should().Contain("has an unsaved Id value");
            Assert.Throws<Exception>(() => repository.ShouldContain(new PolyTypeBuilder().With(e => e.Id, 1).Value())).Message.Should().Contain("Could not find");
            Assert.Throws<Exception>(() => repository.ShouldContain<PolyType>(1)).Message.Should().Contain("Could not find");
        }
    }
}

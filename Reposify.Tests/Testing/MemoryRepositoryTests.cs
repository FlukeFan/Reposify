﻿using System;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryTests : IRepositoryTests
    {
        protected override IRepository<int> New()
        {
            return new MemoryRepository<int>(new ConsistencyInspector());
        }

        [Test]
        public virtual void Flush_DoesNotThrow()
        {
            New().Flush();
        }

        [Test]
        public void ShouldContain()
        {
            var repository = new MemoryRepository<int>(new ConsistencyInspector());
            var entity = new PolyTypeBuilder().Value();

            repository.Save(entity);

            repository.ShouldContain(entity);
            repository.ShouldContain<PolyType>(entity.Id);
        }

        [Test]
        public void ShouldContain_Throws()
        {
            var repository = new MemoryRepository<int>(new ConsistencyInspector());

            Assert.Throws<Exception>(() => repository.ShouldContain(null)).Message.Should().Contain("should not be null");
            Assert.Throws<Exception>(() => repository.ShouldContain(new PolyTypeBuilder().Value())).Message.Should().Contain("has an unsaved Id value");
            Assert.Throws<Exception>(() => repository.ShouldContain(new PolyTypeBuilder().With(e => e.Id, 1).Value())).Message.Should().Contain("Could not find");
            Assert.Throws<Exception>(() => repository.ShouldContain<PolyType>(1)).Message.Should().Contain("Could not find");
        }
    }
}

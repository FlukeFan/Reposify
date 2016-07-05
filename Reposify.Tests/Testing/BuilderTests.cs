using System;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class BuilderTests
    {
        public class Unprotected
        {
            public string StringValue { get; set; }
        }

        public class UnprotectedBuilder : Builder<Unprotected>
        {
            public UnprotectedBuilder() : base(Create<Unprotected>(true), true) { }
        }

        public class Protected
        {
            protected Protected() { }

            public string StringValue   { get; protected set; }
            public string Unprotected   { get; set; }
            public string NoMutator     { get { return StringValue; } }
        }

        public class ProtectedBuilder : Builder<Protected> { }

        public class Mixed
        {
            public string StringValue { get; protected set; }
        }

        public class MixedBuilder : Builder<Mixed>
        {
            public MixedBuilder() : base(Create<Mixed>(true), false) { }
        }

        [Test]
        public void WhenUnprotected_CanInstantiateAndMutate()
        {
            var unprotected = new UnprotectedBuilder()
                .With(u => u.StringValue, "Value")
                .Value();

            unprotected.StringValue.Should().Be("Value");
        }

        [Test]
        public void WhenUnprotected_DefaultBuilderThrows()
        {
            var e = Assert.Throws<Exception>(() =>
            {
                new Builder<Unprotected>();
            });

            e.Message.Should().Contain("default constructor");
        }

        [Test]
        public void WhenProtected_CanInstantiateAndMutate()
        {
            var protectedObj = new ProtectedBuilder()
                .With(u => u.StringValue, "Value")
                .Value();

            protectedObj.StringValue.Should().Be("Value");
        }

        [Test]
        public void WhenMutatorNotProtected_Throws()
        {
            var e = Assert.Throws<Exception>(() =>
            {
                new ProtectedBuilder()
                    .With(u => u.Unprotected, "Value");
            });

            e.Message.Should().Contain("is not protected");
        }

        [Test]
        public void WhenNoMutator_Throws()
        {
            var e = Assert.Throws<Exception>(() =>
            {
                new ProtectedBuilder()
                    .With(u => u.NoMutator, "Value");
            });

            e.Message.Should().Contain("does not have a mutator");
        }

        [Test]
        public void WhenMixed_CanInstantiateAndMutate()
        {
            var mixed = new MixedBuilder()
                .With(u => u.StringValue, "Value")
                .Value();

            mixed.StringValue.Should().Be("Value");
        }

        [Test]
        public void CanMutateExistingInstance()
        {
            var obj = new ProtectedBuilder()
                .With(p => p.StringValue, "before")
                .Value();

            obj.StringValue.Should().Be("before");

            Builder.Modify(obj).With(p => p.StringValue, "after");

            obj.StringValue.Should().Be("after");
        }
    }
}

using System;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class ConsistencyInspectorTests
    {
        public class FakeEntity
        {
            public object       Object              { get; set; }
            public DateTime     DateTime            { get; set; }
            public DateTime?    NullableDateTime    { get; set; }
            public string       String              { get; set; }
        }

        [Test]
        public void WhenMsSql_DateTimeIsValidated()
        {
            var inspector = new ConsistencyInspector();

            Assert.Throws<Exception>(() => inspector.BeforeSave(new FakeEntity { DateTime = DateTime.MinValue }));

            Assert.DoesNotThrow(() => inspector.BeforeSave(new FakeEntity { DateTime = new DateTime(2008, 07, 06) }));
            Assert.DoesNotThrow(() => inspector.BeforeSave(new FakeEntity { DateTime = DateTime.MaxValue }));
        }

        [Test]
        public void WhenNotMsSql_DateTimeIsIgnored()
        {
            var inspector = new ConsistencyInspector(isMsSql: false);

            Assert.DoesNotThrow(() => inspector.BeforeSave(new FakeEntity { DateTime = DateTime.MinValue }));
            Assert.DoesNotThrow(() => inspector.BeforeSave(new FakeEntity { DateTime = new DateTime(2008, 07, 06) }));
            Assert.DoesNotThrow(() => inspector.BeforeSave(new FakeEntity { DateTime = DateTime.MaxValue }));
        }

        [Test]
        public void Check_AllPropertiesValid()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity
            {
                Object = new object(),
                DateTime = new DateTime(2008, 07, 06),
                NullableDateTime = new DateTime(2009, 08, 07),
                String = "not null",
            };

            Assert.DoesNotThrow(() => inspector.Check(() => entity.String, s => s.Length.Should().Be(8)));
            Assert.DoesNotThrow(() => inspector.CheckNotNull(() => entity.Object));
            Assert.DoesNotThrow(() => inspector.CheckMsSqlDateTime(() => entity.DateTime));
            Assert.DoesNotThrow(() => inspector.CheckNotNull(() => entity.NullableDateTime));
            Assert.DoesNotThrow(() => inspector.CheckNotNullOrEmpty(() => entity.String));
            Assert.DoesNotThrow(() => inspector.CheckMaxLength(() => entity.String, 8));
        }

        [Test]
        public void Check_ThrowsWithPropertyName()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { String = null };

            var e = Assert.Throws<Exception>(() => inspector.Check(() => entity.String, s => { throw new Exception("was not null"); }));

            e.Message.Should().Contain("property String is not valid: was not null");
        }

        [Test]
        public void CheckNotNullObject_ThrowsWhenNull()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { Object = null };

            var e = Assert.Throws<Exception>(() => inspector.CheckNotNull(() => entity.Object));

            e.Message.Should().Contain("Object cannot be null");
        }

        [Test]
        public void CheckNotNullValue_ThrowsWhenNull()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { NullableDateTime = null };

            var e = Assert.Throws<Exception>(() => inspector.CheckNotNull(() => entity.NullableDateTime));

            e.Message.Should().Contain("NullableDateTime cannot be null");
        }

        [Test]
        public void CheckNotNullOrEmpty_Throws()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { String = null };

            var e = Assert.Throws<Exception>(() => inspector.CheckNotNullOrEmpty(() => entity.String));

            e.Message.Should().Contain("String cannot be null");
        }

        [Test]
        public void CheckNotNullOrEmpty_ThrowsWhenEmpty()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { String = "" };

            var e = Assert.Throws<Exception>(() => inspector.CheckNotNullOrEmpty(() => entity.String));

            e.Message.Should().Contain("String cannot be empty");
        }

        [Test]
        public void CheckMaxLength_ThrowsWhenTooLarge()
        {
            var inspector = new ConsistencyInspector();
            var entity = new FakeEntity { String = "7 chars" };

            var e = Assert.Throws<Exception>(() => inspector.CheckMaxLength(() => entity.String, 6));

            e.Message.Should().Contain("string property String has length 7 which is larger than the maximum length of 6");
        }
    }
}

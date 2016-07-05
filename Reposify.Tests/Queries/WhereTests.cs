using System;
using FluentAssertions;
using NUnit.Framework;
using Reposify.Queries;

namespace Reposify.Tests.Queries
{
    [TestFixture]
    public class WhereTests
    {
        [Test]
        public void SimpleComparison()
        {
            var where = Where.For<PolyType>(p => p.String == "test string")
                .As<WhereBinaryComparison>();

            where.Operand1.Name.Should().Be("String");
            where.Operator.Should().Be(WhereBinaryComparison.OperatorType.Equal);
            where.Operand2.Should().Be("test string");
        }

        [Test]
        public void NotAllowedExpression()
        {
            var exception = Assert.Throws<Exception>(() => Where.For<PolyType>(p => char.IsNumber(p.String[0])));

            exception.Message.Should().StartWith("Unable to form query for:");
        }
    }
}

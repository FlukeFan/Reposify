using FluentAssertions;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class CustomInspectionsTests
    {
        public class Tree1 { public string Value { get; set; } }

        public class Tree1_1 : Tree1 { }

        public class Tree1_2 : Tree1 { }

        public class Tree1_1_1 : Tree1_1 { public string Value2 { get; set; } }

        [Test]
        public void Inspect_Depth2()
        {
            var inspections = new CustomInspections();
            inspections.AddInspection<Tree1_1_1>((ci, e) => e.Value = "inspected");

            var entity = new Tree1_1_1 { Value = "not inspected" };
            inspections.InspectEntity(entity);

            entity.Value.Should().Be("inspected");
        }

        [Test]
        public void Inspect_NotFound()
        {
            var inspections = new CustomInspections();
            inspections.AddInspection<Tree1_1_1>((ci, e) => e.Value = "inspected");

            var entity = new Tree1_2 { Value = "not inspected" };
            inspections.InspectEntity(entity);

            entity.Value.Should().Be("not inspected");
        }

        [Test]
        public void Inspect_ParentInspection()
        {
            var inspections = new CustomInspections();
            inspections.AddInspection<Tree1>((ci, e) => e.Value = "inspected");
            inspections.AddInspection<Tree1_1_1>((ci, e) => e.Value2 = "inspected");

            var entity = new Tree1_2 { Value = "not inspected" };
            inspections.InspectEntity(entity);

            entity.Value.Should().Be("inspected");
        }

        [Test]
        public void Inspect_MultipleInspections()
        {
            var inspections = new CustomInspections();
            inspections.AddInspection<Tree1>((ci, e) => e.Value = "inspected");
            inspections.AddInspection<Tree1_1_1>((ci, e) => e.Value2 = "inspected");

            var entity = new Tree1_1_1 { Value = "not inspected", Value2 = "not inspected" };
            inspections.InspectEntity(entity);

            entity.Value.Should().Be("inspected");
            entity.Value2.Should().Be("inspected");
        }
    }
}

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using NUnit.Framework;
using Reposify.Database.Tests;
using Reposify.Tests;

namespace Reposify.Ef6.Tests
{
    [Explicit]
    public class Ef6RepositoryTests : IRepositoryTests
    {
        public class TestsDbContext : DbContext
        {
            public TestsDbContext(string connectionString) : base(connectionString)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

                var polyType = modelBuilder.Entity<PolyType>();
                polyType.HasOptional(p => p.SubType).WithOptionalPrincipal().Map(m => m.MapKey("SubType"));
            }
        }

        private static BuildEnvironment _environment;

        static Ef6RepositoryTests()
        {
            _environment = BuildEnvironment.Load();
        }

        protected override IRepository<int> New()
        {
            var dbContext = new TestsDbContext(_environment.Connection);
            return new Ef6Repository<int>(dbContext).Open();
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}

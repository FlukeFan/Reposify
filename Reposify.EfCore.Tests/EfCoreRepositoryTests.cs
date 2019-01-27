using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Reposify.Database.Tests;
using Reposify.Tests;

namespace Reposify.Ef6.Tests
{
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
        private static Ef6Handlers      _handlers;

        static Ef6RepositoryTests()
        {
            _environment = BuildEnvironment.Load();
            _handlers = new Ef6Handlers().UsingHandlersFromAssemblyForType<Ef6RepositoryTests>();
        }

        protected override IRepository New()
        {
            var dbContext = new TestsDbContext(_environment.Connection);
            return new Ef6Repository(dbContext).UsingHandlers(_handlers).Open();
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}

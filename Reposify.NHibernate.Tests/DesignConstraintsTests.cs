using NUnit.Framework;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void DependenciesHaveNotChanged()
        {
            var folder = @"..\..\..\_output";
            var name = "Reposify.NHibernate";

            NugetDependency.VerifyDependencies(folder, name, new string[]
            {
                "Reposify:*",
                "NHibernate:4.1.1.4000",
            });
        }
    }
}

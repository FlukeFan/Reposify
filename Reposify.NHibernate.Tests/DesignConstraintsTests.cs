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

            NugetPackage.VerifyDependencies(folder, name, new string[]
            {
                "Reposify:*",
                "NHibernate:4.0.0.4000",
            });
        }
    }
}

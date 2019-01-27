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
            var name = "Reposify.NHibernate";
            var packageFolder = TestUtil.FindBinConfigFolder(".", name);

            NugetPackage.VerifyDependencies(packageFolder, name, new string[]
                {
                    "Reposify:*",
                    "NHibernate:5.1.0",
                });
        }
    }
}

using NUnit.Framework;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void DependenciesHaveNotChanged()
        {
            var name = "Reposify.Testing";
            var packageFolder = TestUtil.FindBinConfigFolder(".", name);

            NugetPackage.VerifyDependencies(packageFolder, name, new string[]
            {
                "Reposify:*",
            });
        }
    }
}

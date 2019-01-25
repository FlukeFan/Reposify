using NUnit.Framework;

namespace Reposify.Tests
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void DependenciesHaveNotChanged()
        {
            var name = "Reposify";
            var packageFolder = TestUtil.FindBinConfigFolder(".", name);

            NugetPackage.VerifyDependencies(packageFolder, name, new string[]
            {
            });
        }
    }
}

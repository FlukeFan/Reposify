using NUnit.Framework;

namespace Reposify.Tests
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void DependenciesHaveNotChanged()
        {
            var folder = @"..\..\..\_output";
            var name = "Reposify";

            NugetPackage.VerifyDependencies(folder, name, new string[]
            {
            });
        }
    }
}

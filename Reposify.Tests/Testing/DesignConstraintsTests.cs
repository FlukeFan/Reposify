using NUnit.Framework;

namespace Reposify.Tests.Testing
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void DependenciesHaveNotChanged()
        {
            var folder = @"..\..\..\_output";
            var name = "Reposify.Testing";

            NugetPackage.VerifyDependencies(folder, name, new string[]
            {
                "Reposify:*",
            });
        }
    }
}

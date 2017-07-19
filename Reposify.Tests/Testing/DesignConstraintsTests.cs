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

            NugetDependency.VerifyDependencies(folder, name, new string[]
            {
                "Reposify:*",
            });
        }
    }
}

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

            NugetPackage.Find(folder, name)
                .VerifyDependencies(new string[]
                {
                    "Reposify:*",
                });
        }
    }
}

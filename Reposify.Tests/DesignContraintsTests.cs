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

            NugetPackage.Find(folder, name)
                .VerifyDependencies(new string[]
                {
                });
        }
    }
}

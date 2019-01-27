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
                    "NHibernate:4.0.0.4000",
                });

            //TODO: verify what this used to do
                //.VerifyProjectDependencies(@"..\..\..\Reposify.NHibernate\Reposify.NHibernate.csproj", new string[]
                //{
                //    "NHibernate:4.0.0.4000",
                //});
        }
    }
}

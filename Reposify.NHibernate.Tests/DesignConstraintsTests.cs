using NUnit.Framework;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        [Ignore("Not got this working yet - shouldn't be an Iesi.Collections dependency")]
        public void DependenciesHaveNotChanged()
        {
            var folder = @"..\..\..\_output";
            var name = "Reposify.NHibernate";

            NugetDependency.VerifyDependencies(folder, name, new string[]
            {
                "Reposify:*",
                "NHibernate:4.0.0.4000",
            });
        }
    }
}

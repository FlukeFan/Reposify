using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;

namespace Reposify.Tests
{
    [TestFixture]
    public class DesignContraintsTests
    {
        [Test]
        public void PrivateAssemblies()
        {
            var knownPublicReferences = new string[]
            {
                "Microsoft.CSharp",
                "System",
                "System.ComponentModel.DataAnnotations",
                "System.Core",
                "System.Data",
                "System.Xml",
                "System.Xml.Linq",
                "System.Data.DataSetExtensions",
            };

            var packagesFolder = Path.GetFullPath(@"..\..\..\packages");

            var csProjFiles =
                Directory.GetFiles(@"..\..\..", "*.csproj", SearchOption.AllDirectories)
                    .Select(f => Path.GetFullPath(f))
                    .Where(f => !f.StartsWith(packagesFolder))
                    .ToList();

            var nonPrivateReferences = new List<string>();

            foreach (var csProjFile in csProjFiles)
            {
                var projXml = new XmlDocument();
                projXml.Load(csProjFile);

                var references = projXml.SelectNodes("//*[local-name()='Reference']")
                    .Cast<XmlElement>();

                foreach (var reference in references)
                {
                    var assembly = reference.Attributes["Include"].Value;

                    if (knownPublicReferences.Contains(assembly))
                        continue;

                    var privateNode = reference.ChildNodes
                        .Cast<XmlElement>()
                        .Where(n => n.Name == "Private")
                        .SingleOrDefault();

                    if (privateNode == null || privateNode.InnerText != "True")
                        nonPrivateReferences.Add($"{csProjFile} has reference to assembly '{assembly}' that is not marked as private");
                }
            }

            if (nonPrivateReferences.Count > 0)
                Assert.Fail(string.Join("\n", nonPrivateReferences));
        }
    }
}

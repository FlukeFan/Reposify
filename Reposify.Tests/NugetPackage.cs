using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Reposify.Tests
{
    public class NugetPackage
    {
        public static NugetPackage Find(string folder, string name)
        {
            var regFile = name + "\\.\\d+\\.\\d+\\.\\d+(\\.\\d+)?\\.nupkg";
            var regEx = new Regex(regFile);
            var files = Directory.GetFiles(folder, "*.*");
            var archiveFiles = files.Where(f => regEx.IsMatch(f)).ToList();

            if (archiveFiles.Count != 1)
                throw new Exception(string.Format("Expected single archive, but found {0} files:\n{1}\nfrom all files:\n{2}\nusing regex {3}",
                    archiveFiles.Count,
                    string.Join("\n", archiveFiles),
                    string.Join("\n", files),
                    regFile));

            var archiveFile = archiveFiles.Single();
            return new NugetPackage(name, archiveFile);
        }

        private string _name;
        private string _archiveFile;

        protected NugetPackage(string name, string archiveFile)
        {
            _name = name;
            _archiveFile = archiveFile;
        }

        public IList<string> ContentFiles(string folder, string name)
        {
            IList<string> contentFiles = null;

            UsingPackage(archive =>
            {
                contentFiles = archive.Entries
                    .Select(ae => ae.FullName)
                    .Where(e => e.StartsWith("content/"))
                    .ToList();
            });

            return contentFiles;
        }

        public IList<NugetDependency> Dependencies()
        {
            IList<NugetDependency> dependencies = null;

            UsingPackage(archive =>
            {
                var nuspecName = _name + ".nuspec";
                var entries = archive.Entries;
                var nuspecs = entries.Where(e => e.Name == nuspecName).ToList();

                if (nuspecs.Count != 1)
                    throw new Exception(string.Format("Could not find '{0}' in:\n{1}",
                        nuspecName,
                        string.Join("\n", entries.Select(e => e.FullName))));

                var nuspec = nuspecs.Single();

                using (var stream = nuspec.Open())
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);
                    var dependencyNodes = doc.SelectNodes("//*[local-name()='dependencies']/*[local-name()='dependency']");
                    dependencies = dependencyNodes.Cast<XmlElement>().Select(e => new NugetDependency(e)).ToList();
                }
            });

            return dependencies;
        }

        public NugetPackage VerifyDependencies(string[] expectedDependencies)
        {
            var dependencies = Dependencies();

            var extraDependencies = dependencies
                .Where(d => !expectedDependencies.Any(ed => d.Matches(ed)))
                .ToList();

            if (extraDependencies.Count != 0)
                throw new Exception($"Found unexpected dependencies for {_name}:\n{string.Join("\n", extraDependencies)}");

            var missingDependencies = expectedDependencies
                .Where(ed => !dependencies.Any(d => d.Matches(ed)))
                .ToList();

            if (missingDependencies.Count != 0)
                throw new Exception($"Missing dependencies for {_name}:\n{string.Join("\n", missingDependencies)}");

            return this;
        }

        public NugetPackage VerifyProjectDependencies(string projectFile, string[] expectedDependencies)
        {
            var dependencies = Dependencies();

            var project = new XmlDocument();
            project.Load(projectFile);

            var references = project.SelectNodes("//*[local-name()='Reference']");
            var referenceNames = references.Cast<XmlElement>().Select(r => new AssemblyName(r.GetAttribute("Include"))).ToList();

            foreach (var expectedDependency in expectedDependencies)
            {
                var name = expectedDependency.Split(':')[0];
                var version = new Version(expectedDependency.Split(':')[1]);

                var reference = referenceNames.Where(rn => rn.Name == name).SingleOrDefault();

                if (reference != null)
                {
                    if (reference.Version > version)
                        throw new Exception($"Reference to {name} in {projectFile} has version {reference.Version} which is greater than expected version {version}");
                }
            }

            return this;
        }

        private void UsingPackage(Action<ZipArchive> action)
        {
            using (var archive = ZipFile.OpenRead(_archiveFile))
                action(archive);
        }

        public class NugetDependency
        {
            public NugetDependency(XmlElement dependency)
            {
                Id = dependency.Attributes["id"].Value;
                Version = dependency.Attributes["version"].Value;
            }

            public string Id { get; protected set; }
            public string Version { get; protected set; }

            public override string ToString()
            {
                return $"{Id}:{Version}";
            }

            public bool Matches(string expected)
            {
                if (expected.EndsWith(":*"))
                    return Id == expected.Substring(0, expected.Length - 2);
                else
                    return ToString() == expected;
            }
        }
    }
}

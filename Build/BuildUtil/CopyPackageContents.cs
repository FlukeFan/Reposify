using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Build.BuildUtil
{
    public class CopyPackageContents : Command
    {
        public override void Execute(Stack<string> args)
        {
            if (args.Count != 2)
                throw new Exception($"usage: dotnet Build.dll CopyPackageContents <assets_file> <nuget_root>");

            var assetsFile = args.Pop();
            var nugetRoot = args.Pop();

            UsingConsoleColor(ConsoleColor.White, () => Console.WriteLine($"Unzipping package contents assetsFile='{assetsFile}' nuget-root='{nugetRoot}'"));

            var packages = CollectPackages(assetsFile);

            var targetRoot = Path.Combine(Path.GetDirectoryName(assetsFile), "../bin/content");

            if (Directory.Exists(targetRoot))
                Directory.Delete(targetRoot, true);

            Directory.CreateDirectory(targetRoot);

            CopyPackages(nugetRoot, packages, targetRoot);
        }

        private IList<Package> CollectPackages(string assetsFile)
        {
            var packages = new List<Package>();
            var json = File.ReadAllText(assetsFile);
            var assets = (JObject)JsonConvert.DeserializeObject(json);

            var libraries = assets["libraries"];

            foreach (var library in libraries)
            {
                var libProps = library.Children();
                var type = libProps["type"].First().Value<string>();

                if (type != "package")
                    continue;

                var nameParts = library.Value<JProperty>().Name.Split('/');
                var name = nameParts[0];
                var version = nameParts[1];
                var path = libProps["path"].First().Value<string>();

                packages.Add(new Package
                {
                    Name = name,
                    Version = version,
                    Path = path,
                });
            }

            return packages;
        }

        private void CopyPackages(string nugetRoot, IList<Package> packages, string targetRoot)
        {
            foreach (var package in packages)
            {
                var nugetPackage = Path.Combine(nugetRoot, package.Path);
                var targetFolder = Path.Combine(targetRoot, package.Name);
                CopyContent(nugetPackage, "content", targetFolder, package.Version);
                CopyContent(nugetPackage, "contentFiles", targetFolder, package.Version);
            }
        }

        private void CopyContent(string packageFolder, string contentName, string targetFolder, string version)
        {
            var source = Path.Combine(packageFolder, contentName);

            if (Directory.Exists(source))
            {
                CopyFolder(source, Path.Combine(targetFolder, contentName), version);
                UsingConsoleColor(ConsoleColor.Cyan, () => Console.WriteLine($"Copied NuGet package content {source}"));
            }
        }

        private void CopyFolder(string source, string target, string version)
        {
            // https://stackoverflow.com/a/8022011/357728
            // cos it's 2018, and we still have to write code to copy a directory *sigh*

            foreach (string dir in System.IO.Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(Path.Combine(target, dir.Substring(source.Length + 1)));

            foreach (string fileName in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                File.Copy(fileName, Path.Combine(target, fileName.Substring(source.Length + 1).Replace(version, "version")));
        }

        private class Package
        {
            public string Name;
            public string Version;
            public string Path;
        }
    }
}

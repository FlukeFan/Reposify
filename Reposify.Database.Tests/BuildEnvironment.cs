using System;
using System.IO;
using System.Xml;

namespace Reposify.Database.Tests
{
    public class BuildEnvironment
    {
        public string MasterConnection;
        public string Connection;

        public static BuildEnvironment Load()
        {
            var folder = Environment.CurrentDirectory;
            var searchFile = "_output/BuildEnvironment.xml";

            while (!File.Exists(Path.Combine(folder, searchFile)))
            {
                var dir = new DirectoryInfo(folder);

                if (dir.Parent == null)
                    throw new Exception("Could not find file " + searchFile);

                folder = dir.Parent.FullName;
            }

            var buildEnvironmentFile = Path.Combine(folder, searchFile);
            var xml = new XmlDocument();
            xml.Load(buildEnvironmentFile);
            var buildEnvironment = new BuildEnvironment();

            buildEnvironment.MasterConnection = xml.SelectSingleNode("//MasterConnection").InnerText;
            buildEnvironment.Connection = xml.SelectSingleNode("//Connection").InnerText;

            return buildEnvironment;
        }
    }
}

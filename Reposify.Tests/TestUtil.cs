using System;
using System.IO;
using System.Threading.Tasks;

namespace Reposify.Tests
{
    public class TestUtil
    {
        public static string FindBinConfigFolder(string searchFolder, string name)
        {
            var config = "Unknown";
            searchFolder = Path.GetFullPath(searchFolder);

            while (!Directory.Exists(ConfigFolder(searchFolder, name, config)))
            {
                var parent = Directory.GetParent(searchFolder).FullName;

                if (parent == searchFolder)
                    throw new Exception($"Could not find ");

                if (Path.GetFileName(parent)?.ToLower() == "bin")
                    config = Path.GetFileName(searchFolder);

                searchFolder = parent;
            }

            return ConfigFolder(searchFolder, name, config);
        }

        public static string ConfigFolder(string searchFolder, string name, string config)
        {
            return Path.Combine(searchFolder, name, "bin", config);
        }

        public static async Task AssertThrowsAsync(Func<Task> task)
        {
            try
            {
                await task();
                throw new Exception($"Expected exception, but none was thrown");
            } catch
            {
                // success
            }
        }
    }
}

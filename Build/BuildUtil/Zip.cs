using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Build.BuildUtil
{
    public class Zip : Command
    {
        public override void Execute(Stack<string> args)
        {
            if (args.Count != 2)
                throw new Exception($"usage: dotnet Build.dll Zip <folder-to-zip> <zip-file> ");

            var folder = Path.GetFullPath(args.Pop());
            var zipFile = Path.GetFullPath(args.Pop());

            if (File.Exists(zipFile))
                File.Delete(zipFile);

            ZipFile.CreateFromDirectory(folder, zipFile, CompressionLevel.Optimal, false);
            UsingConsoleColor(ConsoleColor.Green, () => Console.WriteLine($"Zipped '{folder}' into '{zipFile}'"));
        }
    }
}

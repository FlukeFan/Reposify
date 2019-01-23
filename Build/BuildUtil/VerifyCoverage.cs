using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Build.BuildUtil
{
    public class VerifyCoverage : Command
    {
        public override void Execute(Stack<string> args)
        {
            if (args.Count != 2)
                throw new Exception($"usage: dotnet Build.dll VerifyCoverage <target%> <coverage file>");

            var targetPercentage = double.Parse(args.Pop());
            var coverageFile = args.Pop();

            var doc = new XmlDocument();
            doc.Load(coverageFile);

            var lineCoverageNode = doc.SelectSingleNode("/CoverageReport/Summary/Linecoverage");
            var numerals = "0123456789";
            var lineCoverageText = string.Concat(lineCoverageNode.InnerText.TakeWhile(c => numerals.Contains(c)));
            var actualLineCoverage = double.Parse(lineCoverageText);

            if (actualLineCoverage < targetPercentage)
                throw new Exception($"Expected at least {targetPercentage}% coverage, only got {actualLineCoverage}% coverage in {Path.GetFullPath(coverageFile)}");

            UsingConsoleColor(ConsoleColor.Green, () => Console.WriteLine($"Coverage of {actualLineCoverage}% is greater than target of {targetPercentage}% "));
        }
    }
}

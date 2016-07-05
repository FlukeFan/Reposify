
// script to verify a coverage summary has passed a given target
// run using WSH (CScript.exe)

var args = WScript.Arguments;

if (args.length != 2)
    throw "Usage: VerifyCoverage.js <target percentage> <coverage summary file>";

var target = args(0);
var summaryFilename = args(1);

var summaryDoc = new ActiveXObject("Microsoft.XMLDOM");
summaryDoc.load(summaryFilename);

var actualNode = summaryDoc.selectSingleNode('/CoverageReport/Summary/Linecoverage');
var actual = actualNode.text;
actual = actual.replace('%', '');

if (Number(actual) < Number(target))
    throw new Error("Expected at least " + target + "% coverage, only got " + actual + "% coverage");

WScript.Echo("Coverage of " + actual + "% is greater than target of " + target + "%");
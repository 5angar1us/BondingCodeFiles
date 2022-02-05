using CommandLine;

namespace BondingCodeFiles
{
    partial class Program
    {
        public class Options
        {
            [Option('i', "infile", Required = true, HelpText = "")]
            public string SourcePath { get; set; }

            [Option('o', "outfile", Required = true, HelpText = "")]
            public string TargetPath { get; set; }
        }
    }
}

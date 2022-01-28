using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BondingCodeFiles
{
    class Program
    {

        public class Options
        {
            [Option('i', "infile", Required = true, HelpText = "")]
            public string sourcePath { get; set; }

            [Option('o', "outfile", Required = true, HelpText = "")]
            public string targetPath { get; set; }
        }

        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(async (Options opts) =>
                {
                    return Run(opts.sourcePath, opts.targetPath);
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        private static int Run(string sourcePath, string targetPath)
        {
            if (Directory.Exists(sourcePath) == false)
            {
                Console.WriteLine("This path does not exist");
                return -1;
            }

            string extensionPattern = "*.cs";

            List<string> ignoreList = new List<string>()
            {
                "Debug",
                "Properties",
                "Designer",
                "AssemblyAttributes"
            };

            var files = Directory.GetFiles(sourcePath, extensionPattern, SearchOption.AllDirectories).ToList();

            files.RemoveAll(file => ignoreList.Exists(ignore => file.Contains(ignore)));

            WriteFiles(files, targetPath);

            return 0;
        }

        private static void WriteFiles(List<string> files, string outputPath)
        {
            var sb = new StringBuilder();

            foreach (var s in files)
            {
                using (var sr = new StreamReader(s))
                {

                    var fileName = $"Файл \"{Path.GetFileName(s)}\"";
                    var text = sr.ReadToEnd();

                    sb.AppendLine(CreateTitle(fileName));
                    sb.AppendLine(text);
                }
            }
            
            using (var sw = new StreamWriter(outputPath))
            {
                sw.WriteLine(sb.ToString());
            }

            
        }

        private static string CreateTitle(string titleText)
        {
            const string titleSeparator = "========";
            return $"{titleSeparator} {titleText} {titleSeparator}";
        }
    }
}

using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BondingCodeFiles
{
    partial class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(async (Options opts) =>
                {
                    return Run(opts.SourcePath, opts.TargetPath);
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

            List<string> ignoreList = GetIgnoreList().ToList();

            List<string> files = Directory.GetFiles(sourcePath, extensionPattern, SearchOption.AllDirectories).ToList();

            files.RemoveAll(file => ignoreList.Exists(ignoreItem => file.Contains(ignoreItem)));

            using (StreamWriter streamWriter = new StreamWriter(targetPath))
            {
                ReportWriter reportWriter = new ReportWriter(streamWriter);
                reportWriter.WriteFiles(files);
            }

            return 0;
        }

        private static IEnumerable<string> GetIgnoreList()
        {
            return new List<string>()
            {
                "Debug",
                "Properties",
                "Designer",
                "AssemblyAttributes"
            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HooverUnlimited.DotNetRtfWriter;

namespace BondingCodeFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            MainWork();

            Console.ReadKey();
        }

        private static void MainWork()
        {
            string folderPath = $@"C:\1.Developing\С#\ИнстрСредства ИС\PrimaryDataAnalysis7";
            string outputFileName = "Code of progect.rtf";
            string outputPath = Path.Combine(folderPath, outputFileName);
            string extensionPattern = "*.cs";

            List<string> ignoreList = new List<string>()
            {
                @"\Debug\",
                "Properties",
                ".Designer.",
            };

            var files = Directory.GetFiles(folderPath, extensionPattern, SearchOption.AllDirectories).ToList();

            files.RemoveAll(file => ignoreList.Exists(ignore => file.Contains(ignore)));

            WriteFile(files, outputPath);
        }

        private static void ConsoleWrite(string folderPath, List<string> files)
        {
            foreach (var s in files)
            {
                string relativePath = GetRelativePath(folderPath, s);
                Console.WriteLine(relativePath);
            }
        }

        private static void WriteFile(List<string> files, string outputPath)
        {
            var doc = new RtfDocument(PaperSize.A4, PaperOrientation.Landscape, Lcid.English);

            // Create fonts and colors for later use
            var times = doc.CreateFont("Times New Roman");
            var courier = doc.CreateFont("Courier New");

            foreach (var s in files)
            {
                using (var sr = new StreamReader(s))
                {
                   
                    var fileName = $"Файл \"{Path.GetFileName(s)}\"";
                    var text = sr.ReadToEnd();
                    
                    addTitle(doc, times, fileName);
                    addText(doc, times, text);

                }
            }
            doc.Save(outputPath);


            var p = new Process { StartInfo = { FileName = outputPath } };
            p.Start();
        }

        private static void addTitle(RtfDocument doc, FontDescriptor times, string title)
        {
            RtfCharFormat fmt = null;

            RtfParagraph par = doc.AddParagraph();

            par.StartNewPage = true;

            par.DefaultCharFormat.Font = times;
            par.Alignment = Align.Center;
            par.SetText(title);

            fmt = par.AddCharFormat(0, par.Text.Length - 1);
            fmt.FontSize = 14;

            fmt = par.AddCharFormat(0, 4);
            fmt.FontStyle.AddStyle(FontStyleFlag.Bold);
            fmt.FontSize = 14;
        }

        private static void addText(RtfDocument doc, FontDescriptor times, string text)
        {
            RtfCharFormat fmt = null;

            RtfParagraph par = doc.AddParagraph();

            par.DefaultCharFormat.Font = times;
            par.Alignment = Align.Left;
            par.SetText(text);

            fmt = par.AddCharFormat(0, par.Text.Length - 1);
            fmt.FontSize = 14;
        }

        private static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{ Path.DirectorySeparatorChar }{ rel }";
            }
            return rel;
        }
    }
}

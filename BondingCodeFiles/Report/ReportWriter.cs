using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondingCodeFiles.Report;

public class ReportWriter
{
    private StreamWriter streamWriter;

    public ReportWriter(StreamWriter streamWriter)
    {
        this.streamWriter = streamWriter;
    }

    public void WriteFiles(string rootPath, IEnumerable<string> filePaths)
    {
        var sb = new StringBuilder();
        const string titleSeparator = "========";


        foreach (var path in filePaths)
        {
            using (var streamReader = new StreamReader(path))
            {

                var text = streamReader.ReadToEnd();


                var relativePath = GetRelativePath(rootPath, path);
                sb.AppendLine(titleSeparator);
                sb.AppendLine($"Файл \"{relativePath}\"");
                sb.AppendLine(text);
            }
        }

        streamWriter.WriteLine(sb.ToString());
    }

    static string GetRelativePath(string rootFolder, string filePath)
    {
        Uri rootUri = new Uri(rootFolder + Path.DirectorySeparatorChar);
        Uri fileUri = new Uri(filePath);

        if (rootUri.Scheme != fileUri.Scheme)
        {
            return filePath;
        }

        Uri relativeUri = rootUri.MakeRelativeUri(fileUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));

        return relativePath;
    }
}

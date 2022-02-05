using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondingCodeFiles
{
    public class ReportWriter
    {
        private StreamWriter streamWriter;

        public ReportWriter(StreamWriter streamWriter)
        {
            this.streamWriter = streamWriter;
        }

        public void WriteFiles(IEnumerable<string> files)
        {
            var sb = new StringBuilder();

            foreach (var s in files)
            {
                using (var streamReader = new StreamReader(s))
                {

                    var fileName = $"Файл \"{Path.GetFileName(s)}\"";
                    var text = streamReader.ReadToEnd();

                    sb.AppendLine(CreateTitle(fileName));
                    sb.AppendLine(text);
                }
            }

            streamWriter.WriteLine(sb.ToString());
        }

        private string CreateTitle(string titleText)
        {
            const string titleSeparator = "========";
            return $"{titleSeparator} {titleText} {titleSeparator}";
        }
    }
}

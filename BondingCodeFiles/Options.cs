using BondingCodeFiles.Report;
using CommandLine;

namespace BondingCodeFiles;

public class Options
{
    [Option('s', "source", Required = true, HelpText = "Укажите путь к исходной папке.")]
    public string SourcePath { get; set; }

    [Option('t', "target", Required = true, HelpText = "Укажите путь к результатному файлу.")]
    public string TargetPath { get; set; }

    [Option('e', "empty-line-mode", Default = EmptyLineMode.PreserveEmpty, HelpText = "Режим обработки пустых строк.")]
    public EmptyLineMode EmptyLineMode { get; set; }
}

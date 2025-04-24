using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BondingCodeFiles.Report;
using CommandLine;
using Spectre.Console;
using static BondingCodeFiles.Program;

namespace BondingCodeFiles;

class Program
{
    static async Task<int> Main(string[] args)
    {

        if (args == null || args.Length == 0)
        {
            AnsiConsole.MarkupLine("[green]Интерактивный режим[/]");
            string sourcePath = AnsiConsole.Ask<string>("Введите [blue]исходный путь[/]:");

            string targetFileName = "all_code.odt";
            string parentDirectory = Path.GetDirectoryName(sourcePath);
            string defaultTargetPath = Path.Combine(parentDirectory, targetFileName);

            string targetPath = AnsiConsole.Ask<string>("Введите [blue]целевой путь[/]:", defaultTargetPath);

            return Run(new Options { SourcePath = sourcePath, TargetPath = targetPath, EmptyLineMode = EmptyLineMode.SingleEmpty});
        }

        // Если аргументы переданы, используем CommandLine.Parser для парсинга
        return await Parser.Default.ParseArguments<Options>(args)
            .MapResult(async (Options opts) =>
            {
                return Run(opts);
            },
            errs => Task.FromResult(-1)); // Неверные аргументы
    }


    public static int Run(Options opts)
    {
        string sourcePath = opts.SourcePath;
        string targetPath = opts.TargetPath;


        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine("Указанный исходный путь не существует");
            return -1;
        }

        // Получаем поддерживаемые расширения файлов
        List<string> extensions = FileSource.GetSupportedExtensions();

        // Получаем список игнорируемых директорий/файлов (или используем значения по умолчанию)
        List<string> ignoreList = FileSource.GetIgnoreList();

        // Собираем файлы по каждому расширению
        var files = GetFiles.GetFilteredFiles(sourcePath, extensions, ignoreList)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        if (TryWriteReport(files, sourcePath, targetPath, opts.EmptyLineMode))
        {
            Console.WriteLine("Операция завершена успешно");
            return 0;
        }
        return -1;
    }

    public static bool TryWriteReport(List<string> files, string sourcePath, string targetPath, EmptyLineMode emptyLineMode)
    {
        // Генерируются два файла: например, один с расширением .docx и один с оригинальным расширением targetPath
        var docxPath = Path.ChangeExtension(targetPath, "docx");

        // Если файлы заблокированы, ждать разблокировки
        UnlockFileIfNeed(docxPath);
        UnlockFileIfNeed(targetPath);

        // Пример записи в файл docx (здесь вызов кастомного класса записи)
        using (FileStream fileStream = new FileStream(docxPath, FileMode.OpenOrCreate))
        {
            DoxcWriter doxcWriter = new DoxcWriter(fileStream);
            doxcWriter.WriteFiles(sourcePath, files, emptyLineMode);
        }

        ODTWriter odtWriter = new ODTWriter(targetPath);
        odtWriter.WriteFiles(sourcePath, files);

        return true;
    }


    // Метод для проверки блокировки файлаф
    private static void UnlockFileIfNeed(string path)
    {
        while (true)
        {
            if (FileOperations.IsFileLocked(path) == false)
            {
                break;
            }
            Console.WriteLine($"{path} заблокирован другим приложением. Пожалуйста, разблокируйте его и нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}


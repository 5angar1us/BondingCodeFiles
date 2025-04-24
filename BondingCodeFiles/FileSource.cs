using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondingCodeFiles;

public static class FileSource
{
    // Получить список игнорируемых файлов из файла ignore.txt (если он существует)
    public static List<string> GetIgnoreList()
    {
        const string ignoreFilePath = "ignore.txt";
        string filePath = GetInDefaultConfigRoute(ignoreFilePath);

        return ReadLinesOrDefault(filePath, new List<string>() { });
    }

    // Получаем список поддерживаемых шаблонов расширений из файла support_extensions.txt или по умолчанию
    public static List<string> GetSupportedExtensions()
    {
        string extensionsFile = "support_extensions.txt";
        string filePath = GetInDefaultConfigRoute(extensionsFile);

        return ReadLinesOrDefault(filePath, new List<string>() { });
    }

    public static List<string> ReadLinesOrDefault(string path, List<string> defaultValue)
    {
        if (File.Exists(path))
        {
            bool IsComment(string line) => line.TrimStart().StartsWith("#");

            return File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line) && IsComment(line) == false)
                .ToList();
        }

        return defaultValue;
    }

    private static string GetInDefaultConfigRoute(string fileName)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string ignoreFilePath = Path.Combine(basePath, "Configs", fileName);

        return ignoreFilePath;
    }
}

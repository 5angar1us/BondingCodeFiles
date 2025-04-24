using System.Text.RegularExpressions;

namespace BondingCodeFiles
{
    internal static class GetFiles
    {
        // TODO
        // Support: "env", ".venv"
        // Not support "env\", ".venv\"
        public static IEnumerable<string> GetFilteredFiles(string sourcePath, IEnumerable<string> extensions, IEnumerable<string> ignorePatterns)
        { // Для хранения директорий, которые нужно обойти
            var directories = new Stack<string>();
            directories.Push(sourcePath);

            while (directories.Count > 0)
            {
                var currentDir = directories.Pop();

                // Получаем относительный путь текущей директории относительно исходной
                string relativeDir = Path.GetRelativePath(sourcePath, currentDir)
                                          .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                // Если для текущей директории задано правило игнорирования – пропускаем её целиком.
                // Нужно учесть, что для корневой директории.relativeDir может быть пустой строкой – в этом случае её не игнорируем.
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    // Добавляем разделитель, чтобы шаблон ".venv/" точно совпадал с директорией ".venv"
                    if (ignorePatterns.Any(pattern => IsMatch(relativeDir, pattern)))
                    {
                        continue;
                    }
                }

                // Получаем файлы директории, фильтруем по расширениям
                foreach (var file in Directory.GetFiles(currentDir))
                {
                    if (extensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Можно добавить дополнительную проверку файла по списку ignore,
                        // если, например, нужны проверки не только для директорий.
                        if (!ShouldIgnore(file, sourcePath, ignorePatterns))
                        {
                            yield return file;
                        }
                    }
                }

                // Добавляем поддиректории для обхода
                foreach (var dir in Directory.GetDirectories(currentDir))
                {
                    // Получаем относительный путь текущей директории относительно исходной
                    string relativeChildDir = Path.GetRelativePath(sourcePath, dir)
                                              .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                    if (ignorePatterns.Any(pattern => IsMatch(relativeChildDir + Path.DirectorySeparatorChar, pattern)))
                    {
                        continue;
                    }
                    else
                    {
                        directories.Push(dir);
                    }


                }
            }
        }

        /// <summary>
        /// Сравнивает строку с шаблоном, который может содержать '*' и '?'.
        /// Преобразует шаблон в регулярное выражение.
        /// </summary>
        private static bool IsMatch(string input, string pattern)
        { // Экранируем спецсимволы в pattern
            string escapedPattern = Regex.Escape(pattern);

            // Заменяем эскейпированное "/" на группу, которая соответствует как "/" так и "\"
            escapedPattern = escapedPattern.Replace("/", "['" + @"/\" + "']");

            // Заменяем шаблонные символы '*' и '?' на соответствующие regex-аналоги.
            string regexPattern = "^" + escapedPattern
                                          .Replace("\\*", ".*")
                                          .Replace("\\?", ".") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Проверяет, должен ли файл быть проигнорирован, сравнивая его относительный путь с шаблонами игнорирования.
        /// </summary>
        private static bool ShouldIgnore(string filePath, string sourcePath, IEnumerable<string> ignorePatterns)
        {
            // Получаем путь файла относительно исходной директории
            string relativePath = Path.GetRelativePath(sourcePath, filePath)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            // Если хотя бы один шаблон совпадает, файл игнорируется.
            return ignorePatterns.Any(pattern => IsMatch(relativePath, pattern));
        }
    }
}
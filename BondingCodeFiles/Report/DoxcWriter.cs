using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BondingCodeFiles.Report;

internal class DoxcWriter
{
    private FileStream fileStream;
    private readonly string headingId = "Heading1";
    private readonly string codeId = "Code";

    public DoxcWriter(FileStream fileStream)
    {
        this.fileStream = fileStream;
    }

    // Основной метод, принимающий режим обработки пустых строк.
    public void WriteFiles(string rootPath, List<string> filePaths, EmptyLineMode mode)
    {
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(fileStream, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            AddStyles(mainPart);

            Body body = new Body();

            foreach (var filePath in filePaths)
            {
                string relativePath = Path.GetRelativePath(rootPath, filePath);
                // Заголовок файла
                Paragraph header = CreateParagraph($"Файл \"{relativePath}\"", true);
                body.Append(header);

                // Считываем все строки файла
                string[] lines = File.ReadAllLines(filePath);

                // Для режима SingleEmpty нужно отслеживать, записана ли уже пустая строка.
                bool previousLineWasEmpty = false;

                foreach (var line in lines)
                {
                    switch (mode)
                    {
                        // Удаляем пустые строки – добавляем только непустые строки
                        case EmptyLineMode.RemoveEmpty:
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Paragraph paragraph = CreateParagraph(line, false);
                                body.Append(paragraph);
                            }
                            break;

                        // Оставляем только одну пустую строку подряд.
                        case EmptyLineMode.SingleEmpty:
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                if (!previousLineWasEmpty)
                                {
                                    // Добавляем пустую строку только один раз
                                    Paragraph emptyParagraph = CreateParagraph(string.Empty, false);
                                    body.Append(emptyParagraph);
                                    previousLineWasEmpty = true;
                                }
                            }
                            else
                            {
                                Paragraph paragraph = CreateParagraph(line, false);
                                body.Append(paragraph);
                                previousLineWasEmpty = false;
                            }
                            break;

                        // Сохраняем строки как есть (в том числе множественные пустые строки)
                        case EmptyLineMode.PreserveEmpty:
                            Paragraph p = CreateParagraph(line, false);
                            body.Append(p);
                            break;
                    }
                }
            }

            mainPart.Document.Append(body);
            mainPart.Document.Save();
        }
    }

    Paragraph CreateParagraph(string sourceText, bool isHeader)
    {
        Paragraph paragraph = new Paragraph();
        Run run = new Run();
        Text text = new Text(sourceText) { Space = SpaceProcessingModeValues.Preserve };
        run.Append(text);
        RunProperties runProperties = new RunProperties(
            new RunFonts { Ascii = "Times New Roman" },
            new FontSize { Val = "20" });

        if (isHeader)
        {
            // Adjust the style as header
            paragraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = headingId });
        }
        else
        {
            // Style for code
            paragraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = codeId });
        }

        run.RunProperties = runProperties;
        paragraph.Append(run);

        return paragraph;
    }



    private void AddStyles(MainDocumentPart mainPart)
    {
        StyleDefinitionsPart stylePart;

        if (mainPart.StyleDefinitionsPart != null)
        {
            stylePart = mainPart.StyleDefinitionsPart;
        }
        else
        {
            stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylePart.Styles = new Styles();
        }
        // Проверяем на существование стиля Heading1
        stylePart.Styles.Elements<Style>().ToList().ForEach(x => Console.WriteLine(x.StyleId));

        if (!stylePart.Styles.Elements<Style>().Any(s => s.StyleId == headingId))
        {
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = headingId,
                CustomStyle = true,
            };

            style.Append(new StyleName() { Val = headingId });
            style.Append(new BasedOn() { Val = "Normal" });
            style.Append(new NextParagraphStyle() { Val = "Normal" });
            style.Append(new UIPriority() { Val = 9 });
            style.Append(new PrimaryStyle());
            style.Append(new Rsid() { Val = "00D4101C" });

            StyleRunProperties styleRunProperties = new StyleRunProperties();
            styleRunProperties.Append(new Bold());
            styleRunProperties.Append(new FontSize() { Val = "28" }); // Размер шрифта для заголовка

            style.Append(styleRunProperties);

            stylePart.Styles.Append(style);
            stylePart.Styles.Save();
        }

        if (!stylePart.Styles.Elements<Style>().Any(s => s.StyleId == codeId))
        {
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = codeId,
                CustomStyle = true,
            };

            style.Append(new StyleName() { Val = codeId });
            style.Append(new BasedOn() { Val = "Normal" });
            style.Append(new NextParagraphStyle() { Val = "Normal" });
            style.Append(new UIPriority() { Val = 9 });
            style.Append(new PrimaryStyle());
            style.Append(new Rsid() { Val = "00D4101C" });

            StyleRunProperties styleRunProperties = new StyleRunProperties();
            styleRunProperties.Append(new FontSize() { Val = "28" }); // Размер шрифта для заголовка

            style.Append(styleRunProperties);

            stylePart.Styles.Append(style);
            stylePart.Styles.Save();
        }
    }
}

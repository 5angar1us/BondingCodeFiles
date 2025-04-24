using System;
using System.Collections.Generic;
using System.IO;
using AODL.Document.TextDocuments;
using AODL.Document.Content.Text;
using AODL.Document.Styles;

namespace BondingCodeFiles.Report;

internal class ODTWriter
{
    private readonly string fileName;
    private readonly EmptyLineMode emptyLineMode;

    public ODTWriter(string fileName, EmptyLineMode emptyLineMode = EmptyLineMode.PreserveEmpty)
    {
        this.fileName = fileName;
        this.emptyLineMode = emptyLineMode;
    }

    public void WriteFiles(string rootPath, IEnumerable<string> filePaths)
    {
        using (TextDocument textDocument = new TextDocument())
        {
            textDocument.New();
            AddStyles(textDocument);

            foreach (var filePath in filePaths)
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                    continue;

                string relativePath = Path.GetRelativePath(rootPath, filePath);
                Paragraph header = CreateParagraph($"Файл \"{relativePath}\"", true, textDocument);
                textDocument.Content.Add(header);

                // Используется для режима SingleEmpty – флаг, что предыдущая строка была пустой.
                bool previousLineEmpty = false;

                foreach (var line in lines)
                {
                    bool isEmpty = string.IsNullOrWhiteSpace(line);

                    switch (emptyLineMode)
                    {
                        case EmptyLineMode.RemoveEmpty:
                            if (isEmpty)
                                continue; // пропустить пустые строки
                                          // Если строка не пустая – создаём параграф
                            textDocument.Content.Add(CreateParagraph(line, false, textDocument));
                            break;

                        case EmptyLineMode.SingleEmpty:
                            if (isEmpty)
                            {
                                // Добавляем пустую строку только если предыдущей не было пустой
                                if (!previousLineEmpty)
                                {
                                    textDocument.Content.Add(CreateParagraph(string.Empty, false, textDocument));
                                    previousLineEmpty = true;
                                }
                                // Если уже добавлена пустая строка подряд, то пропускаем
                            }
                            else
                            {
                                textDocument.Content.Add(CreateParagraph(line, false, textDocument));
                                previousLineEmpty = false;
                            }
                            break;

                        case EmptyLineMode.PreserveEmpty:
                            // Сохраняем строки как есть (в том числе пустые)
                            textDocument.Content.Add(CreateParagraph(line, false, textDocument));
                            break;
                    }
                }
            }

            textDocument.SaveTo(fileName);
        }
    }

    private Paragraph CreateParagraph(string text, bool isHeader, TextDocument textDocument)
    {
        Paragraph paragraph = new Paragraph(textDocument);
        paragraph.TextContent.Add(new SimpleText(textDocument, text));

        if (isHeader)
            paragraph.StyleName = "Heading1";
        else
            paragraph.StyleName = "Standard";

        return paragraph;
    }

    private void AddStyles(TextDocument textDocument)
    {
        ParagraphStyle headerStyle = new ParagraphStyle(textDocument, "Heading1");
        headerStyle.TextProperties.FontName = "Times New Roman";
        headerStyle.TextProperties.FontSize = "14pt"; // Заголовок
        headerStyle.TextProperties.Bold = "bold";
        textDocument.Styles.Add(headerStyle);

        ParagraphStyle standardStyle = new ParagraphStyle(textDocument, "Standard");
        standardStyle.TextProperties.FontName = "Times New Roman";
        standardStyle.TextProperties.FontSize = "12pt"; // Обычный текст
        textDocument.Styles.Add(standardStyle);
    }
}

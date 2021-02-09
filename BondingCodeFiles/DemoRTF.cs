using HooverUnlimited.DotNetRtfWriter;

namespace BondingCodeFiles
{
    class DemoRTF
    {
        private static void Demo(string outputPath)
        {
            // Create document by specifying paper size and orientation,
            // and default language.
            var doc = new RtfDocument(PaperSize.A4, PaperOrientation.Landscape, Lcid.English);

            // Create fonts and colors for later use
            var times = doc.CreateFont("Times New Roman");
            var courier = doc.CreateFont("Courier New");
            var red = doc.CreateColor(new RtfColor("ff0000"));
            var blue = doc.CreateColor(new RtfColor(0, 0, 255));
            var white = doc.CreateColor(new RtfColor(255, 255, 255));
            var colourTableHeader = doc.CreateColor(new RtfColor("76923C"));
            var colourTableRow = doc.CreateColor(new RtfColor("D6E3BC"));
            var colourTableRowAlt = doc.CreateColor(new RtfColor("FFFFFF"));


            RtfParagraph par;
            // Don't instantiate RtfCharFormat by using ``new'' keyword, either.
            // An AddCharFormat method are provided by RtfParagraph objects.
            RtfCharFormat fmt;


            // ==========================================================================
            // Demo 1: Font Setting
            // ==========================================================================
            // If you want to use Latin characters only, it is as simple as assigning
            // ``Font'' property of RtfCharFormat objects. If you want to render Far East
            // characters with some font, and Latin characters with another, you may
            // assign the Far East font to ``Font'' property and the Latin font to
            // ``AnsiFont'' property.
            par = doc.AddParagraph();
            par.Alignment = Align.Left;
            par.DefaultCharFormat.Font = times;
            par.DefaultCharFormat.AnsiFont = courier;
            par.SetText("Testing\n");


            // ==========================================================================
            // Demo 2: Character Formatting
            // ==========================================================================
            par = doc.AddParagraph();
            par.DefaultCharFormat.Font = times;
            par.SetText("Demo2: Character Formatting");
            // Besides setting default character formats of a paragraph, you can specify
            // a range of characters to which formatting is applied. For convenience,
            // let's call it range formatting. The following section sets formatting
            // for the 4th, 5th, ..., 8th characters in the paragraph. (Note: the first
            // character has an index of 0)
            fmt = par.AddCharFormat(4, 8);
            fmt.FgColor = blue;
            fmt.BgColor = red;
            fmt.FontSize = 18;
            // Sets another range formatting. Note that when range formatting overlaps,
            // the latter formatting will overwrite the former ones. In the following,
            // formatting for the 8th chacacter is overwritten.
            fmt = par.AddCharFormat(8, 10);
            fmt.FontStyle.AddStyle(FontStyleFlag.Bold);
            fmt.FontStyle.AddStyle(FontStyleFlag.Underline);
            fmt.Font = courier;


            // ==========================================================================
            // Demo 3: Footnote
            // ==========================================================================
            par = doc.AddParagraph();
            par.SetText("Demo3: Footnote");
            // In this example, the footnote is inserted just after the 7th character in
            // the paragraph.
            par.AddFootnote(7).AddParagraph().SetText("Footnote details here.");


            // ==========================================================================
            // Demo 4: Header and Footer
            // ==========================================================================
            // You may use ``Header'' and ``Footer'' properties of RtfDocument objects to
            // specify information to be displayed in the header and footer of every page,
            // respectively.
            par = doc.Footer.AddParagraph();
            par.SetText("Demo4: Page: / Date: Time:");
            par.Alignment = Align.Center;
            par.DefaultCharFormat.FontSize = 15;
            // You may insert control words, including page number, total pages, date and
            // time, into the header and/or the footer.
            par.AddControlWord(12, RtfFieldControlWord.FieldType.Page);
            par.AddControlWord(13, RtfFieldControlWord.FieldType.NumPages);
            par.AddControlWord(19, RtfFieldControlWord.FieldType.Date);
            par.AddControlWord(25, RtfFieldControlWord.FieldType.Time);
            // Here we also add some text in header.
            par = doc.Header.AddParagraph();
            par.SetText("Demo4: Header");


            // ==========================================================================
            // Demo 8: New page
            // ==========================================================================
            par = doc.AddParagraph();
            par.StartNewPage = true;
            par.SetText("Demo8: New page");


            // ==========================================================================
            // Save
            // ==========================================================================
            // You may also retrieve RTF code string by calling to render() method of
            // RtfDocument objects.
            doc.Save(outputPath);

        }
    }
}

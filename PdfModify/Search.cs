using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Data;

namespace PdfModificationx
{
    /// <summary>
    /// Class for Searching phrases in PDF files
    /// </summary>
    public static class Search
    {
        static String _prevDoubleText;

        static float _prevTopRight; //previous y coordinate
        static Int32 _redWords = 0; //sum of red words
        static Int32 pdfPages = 0; //current PDF page that is being checked
        static Int32 _prevIcislo = 0;
        static Boolean _badFootnoteFound = false; //bad footnote found, return value to Program
        static Boolean _doFootnoteCheck = false; //variable for executing BadFootnote function

        //Storage of each footnote number/y location on page/page
        static DataTable dt;
        static DataRow dr;

        static int pageCounter;

        //current font size of read text - Footnote
        static decimal currentFontSize;
        //current value of read text - Footnote
        static int currentFootnoteValue;

        static int replaceCounter;
        static Dictionary<int, float> dictionary = new Dictionary<int, float>();

        public static int SearchFile(string path)
        {
            SetPDFPagesCount(path);
            _redWords = 0;

            PdfReader reader = new PdfReader(path);
            TextWithFontExtractionStategy S = new TextWithFontExtractionStategy();
            for (int i = 1; i < pdfPages + 1; i++)
            {
                //Console.WriteLine("Analyzing page: " + i.ToString());
                string F = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, S);
            }

            return _redWords;
        }
        public static bool BadFootnote(string path)
        {
            bool result = true;

            _badFootnoteFound = false;

            //Console.WriteLine(Path.GetFileName(path)); //vypsat prvě analyzovaný dokument

            GetFootnote(path);

            result = _badFootnoteFound;
            return result;
        }
        private static void SetPDFPagesCount(string path)
        {
            PdfReader pdfReader = new PdfReader(path);
            pdfPages = pdfReader.NumberOfPages;

        }
        /// <summary>
        /// This method looks for specific text in pdf. Returns true if found. False if not found.
        /// </summary>
        public static bool SearchFile(string path, string searchText)
        {
            SetPDFPagesCount(path);
            bool result = false;
            result = FindText(path, searchText);

            return result;
        }
        /// <summary>
        /// This method looks for number of phrases with fontcolor RED and for text match. Returns string composed of phrases count and bool if text matched.
        /// </summary>
        public static string SearchFile(string path, string searchText, bool getRedPhrasesCount)
        {
            SetPDFPagesCount(path);
            _redWords = 0;
            if (getRedPhrasesCount == true)
            {
                string rstring = "Number of red phrases: ";

                PdfReader reader = new PdfReader(path);
                TextWithFontExtractionStategy S = new TextWithFontExtractionStategy();
                for (int i = 1; i < pdfPages + 1; i++)
                {
                    //Console.WriteLine("Analyzing page: " + i.ToString());
                    string F = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, S);
                }
                //Console.WriteLine(F);
                rstring = rstring + _redWords.ToString();
                bool IsMatched = SearchFile(path, searchText);
                if (IsMatched == false)
                {
                    rstring = rstring + ". Match text NOT found.";
                }
                else
                {
                    rstring = rstring + ". Match text found.";
                }
                return rstring;
            }
            return "";
        }


        private static bool FindText(string path, string searchText)
        {
            bool result = false;
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {

                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                if (text.ToString().Contains(searchText))
                {
                    result = true;
                }

            }
            return result;
        }
        private static void GetFootnote(string path)
        {

            currentFontSize = 0;
            currentFootnoteValue = 0;
            _prevIcislo = 0;

            _prevDoubleText = "";
            _prevTopRight = (float)0;

            replaceCounter = 0;
            _doFootnoteCheck = true;

            dt = new DataTable();
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("Position", typeof(float));
            dt.Columns.Add("Page", typeof(float));

            SetPDFPagesCount(path);
            PdfReader reader2 = new PdfReader(path);
            TextWithFontExtractionStategy S = new TextWithFontExtractionStategy();

            //clear for double badfootnote check issue from 15-10-2014
            dt.Rows.Clear();
            dictionary.Clear();
            //Console.WriteLine("Doc START");
            for (int i = 1; i < pdfPages + 1; i++)
            {

                pageCounter = i;
                //Console.WriteLine("Analyzing page: " + i.ToString());
                string F = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader2, i, S);

            }

            int prev = 0;
            DataView dv = dt.DefaultView;

            dv.Sort = " Page asc, position desc";
            DataTable sortedDT = dv.ToTable();

            foreach (DataRow row in sortedDT.Rows)
            {

                if (prev + 1 == Int32.Parse(row[0].ToString()) || prev == Int32.Parse(row[0].ToString()))
                {


                }
                else
                {
                    _badFootnoteFound = true;
                }
                prev = Int32.Parse(row[0].ToString());

            }
            //Console.WriteLine("Doc END");

            /*
            foreach (DataRow row in sortedDT.Rows)
            {
                Console.WriteLine(row[0].ToString() + " --- " + row[1].ToString() + " --- " + row[2].ToString());

            }
            */

            dictionary.Clear();
            dt.Clear();

            prev = 0;
            _prevIcislo = 0;
            replaceCounter = 0;

        }

        public class TextWithFontExtractionStategy : iTextSharp.text.pdf.parser.ITextExtractionStrategy
        {
            //HTML buffer
            private StringBuilder result = new StringBuilder();

            //Store last used properties
            private Vector lastBaseLine;
            private string lastFont;
            private float lastFontSize;

            //http://api.itextpdf.com/itext/com/itextpdf/text/pdf/parser/TextRenderInfo.html
            private enum TextRenderMode
            {
                FillText = 0,
                StrokeText = 1,
                FillThenStrokeText = 2,
                Invisible = 3,
                FillTextAndAddToPathForClipping = 4,
                StrokeTextAndAddToPathForClipping = 5,
                FillThenStrokeTextAndAddToPathForClipping = 6,
                AddTextToPaddForClipping = 7
            }



            public void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo)
            {
                string curFont = renderInfo.GetFont().PostscriptFontName;
                string curColor = "";
                try
                {
                    curColor = renderInfo.GetFillColor().ToString();
                }
                catch (Exception) { }
                curColor = curColor.Replace("Color value[", "").Replace("]", "");
                //Console.WriteLine(curColor);
                //string curColor = renderInfo.GetStrokeColor().RGB.ToString();
                //Check if faux bold is used
                if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText))
                {
                    curFont += "-Bold";
                }

                //This code assumes that if the baseline changes then we're on a newline
                Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();
                Vector topRight = renderInfo.GetAscentLine().GetEndPoint();


                iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(curBaseline[Vector.I1], curBaseline[Vector.I2], topRight[Vector.I1], topRight[Vector.I2]);
                Single curFontSize = rect.Height;

                if (_doFootnoteCheck == true)
                {
                    string text = renderInfo.GetText();

                    //cislo < 3.92M && cislo > 3.8M
                    //cislo == 3.9104M || cislo == 3.910416M
                    if (Decimal.TryParse(curFontSize.ToString(), out currentFontSize) && (currentFontSize == 3.890762M))
                    {
                        string s = " ";
                        if (text == "1," || text == "2," || text == "3," || text == "4," || text == "5," || text == "6," || text == "7," ||
                           text == "1" || text == "2" || text == "3" || text == "4" || text == "5" || text == "6" || text == "7")
                        {
                            //Console.WriteLine(text);

                            if (_prevDoubleText.Length > 1)
                            {
                                s = _prevDoubleText.Substring(0, 1);
                            }
                            if (_prevDoubleText.Length == 2 && s == text && topRight[1] == _prevTopRight)
                            {
                                _badFootnoteFound = true;
                            }
                            _prevDoubleText = text;
                            _prevTopRight = topRight[1];
                        }
                    }

                    //if (Decimal.TryParse(curFontSize.ToString(), out cislo) && (cislo > 3.5M || cislo < 4M) && !string.IsNullOrWhiteSpace(text) && Int32.TryParse(text, out icislo))
                    if (Decimal.TryParse(curFontSize.ToString(), out currentFontSize) && (currentFontSize == 3.9104M || currentFontSize == 3.910416M || currentFontSize == 3.910412M || currentFontSize == 3.890762M) && !string.IsNullOrWhiteSpace(text) && Int32.TryParse(text, out currentFootnoteValue))
                    {
                        if (topRight[1] > 0 && topRight[1] < 700)
                        {
                            //Console.WriteLine(pageCounter);
                            /*
                            Console.WriteLine("------------------------------------------");
                            Console.WriteLine(curFontSize);
                            Console.WriteLine("page:" + pageCounter);
                            Console.WriteLine("txt: " + text);
                            Console.WriteLine("prv: " + prev_icislo);
                            Console.WriteLine("trgh: " + topRight[1]);
                            Console.WriteLine("------------------------------------------");
                            */
                            if (!dictionary.ContainsKey(currentFootnoteValue))
                            {

                                dictionary.Add(currentFootnoteValue, (float)topRight[1]);

                                dr = dt.NewRow();
                                dr["Number"] = currentFootnoteValue;
                                dr["Position"] = (float)topRight[1];
                                dr["Page"] = (int)pageCounter;
                                dt.Rows.Add(dr);
                            }
                            else
                            {
                                if (replaceCounter < 10)
                                {
                                    dictionary[currentFootnoteValue] = (float)topRight[1];
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        if (dt.Rows[i][0].ToString() == currentFootnoteValue.ToString().Trim())
                                        {
                                            dt.Rows[i][1] = (float)topRight[1];
                                            dt.Rows[i][2] = pageCounter;
                                        }
                                    }
                                    dr = dt.NewRow();
                                    dr["Number"] = currentFootnoteValue;
                                    dr["Position"] = (float)topRight[1];
                                    dr["Page"] = (int)pageCounter;
                                    dt.Rows.Add(dr);
                                    replaceCounter++;
                                }
                            }
                        }
                        _prevIcislo = currentFootnoteValue;

                    }

                }
                if (curColor == "FFFF0000")
                {
                    //Console.WriteLine("Red detected!");
                    //Console.WriteLine(curColor);
                    string s = renderInfo.GetText();
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        //Console.WriteLine(s);
                    }
                    else
                    {
                        _redWords++;
                    }
                }
                //See if something has changed, either the baseline, the font or the font size
                if ((this.lastBaseLine == null) || (curBaseline[Vector.I2] != lastBaseLine[Vector.I2]) || (curFontSize != lastFontSize) || (curFont != lastFont))
                {
                    //if we've put down at least one span tag close it
                    if ((this.lastBaseLine != null))
                    {
                        this.result.AppendLine("</span>");
                    }
                    //If the baseline has changed then insert a line break
                    if ((this.lastBaseLine != null) && curBaseline[Vector.I2] != lastBaseLine[Vector.I2])
                    {
                        this.result.AppendLine("<br />");
                    }
                    //Create an HTML tag with appropriate styles

                    this.result.AppendFormat("<span style=\"font-family:{0};font-size:{1}\font-color:{2}>", curFont, curFontSize, curColor);
                }

                //Append the current text
                this.result.Append(renderInfo.GetText());

                //Set currently used properties
                this.lastBaseLine = curBaseline;
                this.lastFontSize = curFontSize;
                this.lastFont = curFont;


            }

            public string GetResultantText()
            {
                //If we wrote anything then we'll always have a missing closing tag so close it here
                if (result.Length > 0)
                {
                    result.Append("</span>");
                }
                return result.ToString();
            }

            //Not needed
            public void BeginTextBlock() { }
            public void EndTextBlock() { }
            public void RenderImage(ImageRenderInfo renderInfo) { }
        }
    }
}

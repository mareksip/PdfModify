using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using iTextSharp.text;
using System.Drawing;

namespace PdfModificationx
{
    /// <summary>
    /// Class for Writing to PDF file
    /// </summary>
    public static class Write
    {
        static Int32 redWords = 0;
        static Int32 pdfPages = 0;
        private static void SetPDFPagesCount(string path)
        {
            PdfReader pdfReader = new PdfReader(path);
            pdfPages = pdfReader.NumberOfPages;
            //redWords = 0;
        }
        /// <summary>
        /// Converting Pdf to PDF VERSION 1.7
        /// </summary>
        public static void PdfConversion(string path, string savePath)
        {
            PdfReader reader = new PdfReader(path);

            PdfStamper stamper = new PdfStamper(reader, new FileStream(savePath , FileMode.Create), PdfWriter.VERSION_1_7);

            stamper.FormFlattening = true;
            List<int> pagesToReplace = new List<int>();

         
            stamper.Close();
        }

        public static bool writeText(string originalPDFpath, string newPDFpath, string valueToWrite, int writeOnPage, string fontFamily, decimal fontSize, string fontColorHex, int coordinateX, int coordinateY)
        {
            bool res = false;
            SetPDFPagesCount(originalPDFpath);
            //string s = Path.GetDirectoryName(originalPDFpath);


            /*  File.Copy(originalPDFpath, newPDFpath);
              Color color = System.Drawing.ColorTranslator.FromHtml(fontColorHex);
              BaseColor bs = new BaseColor(color);
              iTextSharp.text.Font arial = FontFactory.GetFont("Arial", 50, BaseColor.BLUE);
              iTextSharp.text.Font font = FontFactory.GetFont(fontFamily,Int32.Parse(fontSize.ToString()), bs);
              Phrase phrase = new Phrase(writeValue, arial);
              Document doc = new Document();
              iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                  new System.IO.FileStream(newPDFpath,
                      System.IO.FileMode.Open));
              doc.Open();
              PdfContentByte canvas = writer.DirectContent;
              ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, phrase, coordinateX, coordinateY,1);*/

            PdfReader reader = new PdfReader(originalPDFpath);
            iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // open the writer
            FileStream fs = new FileStream(newPDFpath, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            // the pdf content
            PdfContentByte cb = writer.DirectContent;
            PdfContentByte cb2 = writer.DirectContent;

            Color color = System.Drawing.ColorTranslator.FromHtml(fontColorHex);
            BaseColor bs = new BaseColor(color);
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, false);
            // select the font properties

         

            // create the new page and add it to the pdf

            for (int i = 1; i < pdfPages + 1; i++)
            {
                if (i == writeOnPage)
                {


                    document.NewPage();
                    PdfImportedPage page = writer.GetImportedPage(reader, writeOnPage);
                    int curRot = reader.GetPageRotation(i);

                    cb.SetColorFill(bs);

                    cb.SetFontAndSize(bfTimes, Int32.Parse(fontSize.ToString()));

                    // write the text in the pdf content
                    cb.BeginText();
                    // put the alignment and coordinates here
                    cb.ShowTextAligned(1, valueToWrite, coordinateX, coordinateY, 0);
                    cb.EndText();
                    switch (curRot)
                    {
                        case 0:
                            writer.DirectContent.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            break;

                        case 90:
                            writer.DirectContent.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                            break;

                        case 180:
                            writer.DirectContent.AddTemplate(page, -1f, 0, 0, -1f, reader.GetPageSizeWithRotation(i).Width, reader.GetPageSizeWithRotation(i).Height);
                            break;

                        case 270:
                            writer.DirectContent.AddTemplate(page, 0, 1f, -1f, 0, reader.GetPageSizeWithRotation(i).Width, 0);
                            break;
                    }
                  
                }
                else
                {
                    if (i > 1)
                    {
                        document.NewPage();
                    }
                    PdfImportedPage page2 = writer.GetImportedPage(reader, i);
                    cb2.AddTemplate(page2, 0, 0);

                }
            }

            // close the streams and voilá the file should be changed :)
            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();


            /*
            //creates empty pdf
            Document document = new Document();

            FileStream fs = new FileStream(originalPDFpath, FileMode.OpenOrCreate);
            PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(new Paragraph("Hello World"));
            document.Add(new Paragraph(DateTime.Now.ToString()));
     
            document.Close();
            fs.Close();
            */
            return res;
        }
    }
}

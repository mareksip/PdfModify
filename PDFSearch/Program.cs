using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PDFSearchLib;
using PDFWriteLib;

namespace PDFSearch
{
    class Program
    {
        static Int32 redWords = 0;
        static Int32 pdfPages = 0;
        static void Main(string[] args)
        {

            //bool res = PDFWriteClass.writeText(path, newFilePath, "SAMPLE NEW INPUT TEXT", 1,"",d,"#FF0000",660, 560);

            string path = @"C:\Users\marek\Documents\Projekty\freelancer\vw391686vw\redtext-fail-03-11-2014";
            string[] filePaths = Directory.GetFiles(path, "*.pdf",
                                         SearchOption.TopDirectoryOnly);

            foreach (string st in filePaths)
            {
                string searchText = "A_FundName";

                PdfReader pdfReader = new PdfReader(st);
                pdfPages = pdfReader.NumberOfPages;

                //Console.WriteLine("Analyzing document: " + Path.GetFileName(st));
                bool t = PdfSearchClass.searchPDF(st, searchText);
               
                if(t == true)
                {
                  
                    Console.WriteLine("Search text found: " + t.ToString());
                }
                int i = PdfSearchClass.searchPDF(st);
                if(i> 0)
                {
                    if (t == false)
                    {
                        Console.WriteLine(st);
                    }
                    Console.WriteLine("Red text: " + i.ToString());
                }
                string s = PdfSearchClass.searchPDF(st, searchText, true);
                if(t == true || i > 0)
                { 
                    Console.WriteLine("Result: " + s);
                }

                bool r = PdfSearchClass.BadFootnote(st);
                if (r == true)
                    Console.WriteLine("Bad footnote: " + Path.GetFileName(st));
                
               
            }
            Console.Read();
            
        }


        public static void writeOnLocation(int x, int y)
        {

        }

        /// <summary>
        /// This method looks for number of phrases with fontcolor RED in pdf. Returns number of matches.
        /// </summary>

    }
}

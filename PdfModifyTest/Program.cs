﻿using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfModificationx;

namespace PDFSearch
{
    class Program
    {
        static Int32 redWords = 0;
        static Int32 pdfPages = 0;
        static void Main(string[] args)
        {

            //bool res = PDFWriteClass.writeText(path, newFilePath, "SAMPLE NEW INPUT TEXT", 1,"",d,"#FF0000",660, 560);

            string path = @"C:\Users\marek.ARR\Documents\Projekty\_freelancing\_software\vw391686vw\pdfs\table-extraction";
            string[] filePaths = Directory.GetFiles(path, "*.pdf",
                                         SearchOption.TopDirectoryOnly);
            string desiredPhrase = "noteExpRatFootnote";

            foreach (string st in filePaths)
            {

                //PdfReader pdfReader = new PdfReader(st);
                //pdfPages = pdfReader.NumberOfPages;

                Console.WriteLine("Analyzing document: " + Path.GetFileName(st));
                /*
                bool t = Search.SearchFile(st, desiredPhrase);
               
                if(t == true)
                {
                    Console.WriteLine("Search text found: " + t.ToString());
                }
                int i = Search.SearchFile(st);
                if(i> 0)
                {
                    if (t == false)
                    {
                        Console.WriteLine(st);
                    }
                    Console.WriteLine("Red text: " + i.ToString());
                }
                */

                string s = Search.SearchFile(st, desiredPhrase, true);
                Console.WriteLine("Result: " + s);

                bool r = Search.BadFootnote(st);
                if (r == true)
                {
                    Console.WriteLine("Bad footnote: " + Path.GetFileName(st));
                }

            }
            /*
            string path1 = @"C:\Users\marek\Documents\Projekty\freelancer\vw391686vw\pdf-conversions\Mavis-Tire-Supply-LLC-401k-Plan.pdf";
            string savePath = @"C:\Users\marek\Documents\Projekty\freelancer\vw391686vw\pdf-conversions\opt1.pdf";
            PDFWriteLib.PdfWrite.PdfConversion(path1, savePath);
            */

            Console.Read();
        }
    }
}

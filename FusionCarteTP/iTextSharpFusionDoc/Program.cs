using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;

namespace iTextSharpFusionDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            //string inputFile;
            //string overlayFile;
            //string outFile;
            //Set_Files(args, out inputFile, out overlayFile, out outFile);
            //DoFusion(inputFile,overlayFile,outFile);

            string path = ConfigurationManager.AppSettings["input"];
            var files = Directory.EnumerateFiles(path, "*.pdf");

            if (files.Any())
            {
                foreach (var f in files)
                {
                    DoReorder(f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DoReorder(string inputFile)
        {
            //var inputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Test.pdf");
            //var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Output.pdf");
            var output = Path.Combine(ConfigurationManager.AppSettings["output"], Path.GetFileName(inputFile));

            //Bind a reader to our input file
            var reader = new PdfReader(inputFile);

            //Create our output file, nothing special here
            using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (Document doc = new Document(reader.GetPageSizeWithRotation(1)))
                {
                    //Use a PdfCopy to duplicate each page
                    using (PdfCopy copy = new PdfCopy(doc, fs))
                    {
                        doc.Open();
                        copy.SetLinearPageMode();
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                        //Reorder pages
                        copy.ReorderPages(new int[] { 2, 1 });
                        doc.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="overlayFile"></param>
        /// <param name="outFile"></param>
        private static void DoFusion(string inputFile, string overlayFile, string outFile)
        {
            //Create the reader and document to read the origanl PDF document
            PdfReader reader = new PdfReader(inputFile);
            Document inputDoc = new Document(reader.GetPageSizeWithRotation(1));

            using (FileStream fs = new FileStream(outFile, FileMode.Create))
            {
                //Create the PDF Writer to create the new PDF Document
                PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                inputDoc.Open();
                //Create the Content Byte to stamp to the wrtiter
                PdfContentByte cb1 = outputWriter.DirectContent;

                //Get the PDF document to use as overlay
                PdfReader overlayReader = new PdfReader(overlayFile);
                PdfImportedPage overLay = outputWriter.GetImportedPage(overlayReader, 1);

                //Get the overlay page rotation
                int overlayRotation = overlayReader.GetPageRotation(1);

                int n = reader.NumberOfPages;

                int i = 1;
                while (i <= n)
                {
                    //Make sure the new page's page size macthes the original document
                    inputDoc.SetPageSize(reader.GetPageSizeWithRotation(i));
                    inputDoc.NewPage();


                    PdfImportedPage page = outputWriter.GetImportedPage(reader, i);
                    int rotation = reader.GetPageRotation(i);

                    //Insert the overlay with correct rotation
                    if (overlayRotation == 90 || overlayRotation == 270)
                    {
                        cb1.AddTemplate(overLay, 0, -1f, 1f, 0, 0,
                            reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        //cb1.AddTemplate(overLay, 1f, 0, 0, 1f, 0, 0);
                        cb1.AddTemplate(overLay, -2, 5, true);
                    }

                    //Insert the original PDF page with correct rotation
                    if (rotation == 90 || rotation == 270)
                    {
                        cb1.AddTemplate(page, 0, -1f, 1f, 0, 0,
                            reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        //cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        cb1.AddTemplate(page, 0, 0, true);
                    }

                    //Increment the page
                    i++;
                }
                //Close the input file
                inputDoc.Close();
                //Close the reader for the overlay file
                overlayReader.Close();
            }
            reader.Close();
        }

        /// <summary>
        /// Set the Input, Overlay and Output files from the command line arguments
        /// </summary>
        /// <param name="args">The original command arguments</param>
        /// <param name="inputFile">The orignal PDF document to have the background inserted into</param>
        /// <param name="overlayFile">The PDF document with the overlay image/content</param>
        /// <param name="outFile">The output file to write to</param>
        private static void Set_Files(string[] args, out string inputFile, out string overlayFile, out string outFile)
        {
            //inputFile = args[0];
            //overlayFile = args[1];
            //outFile = args[2];

            inputFile = Path.Combine(ConfigurationManager.AppSettings["output"], "DC_Molitor_98539868_B_0001_20160120_054336_F - Page 1_tempfile.pdf");
            overlayFile = Path.Combine(ConfigurationManager.AppSettings["model"], "C303 - Page 2_tempfile.pdf");
            outFile = Path.Combine(ConfigurationManager.AppSettings["output"], "sample.pdf");

            if (inputFile.Contains("\\") == false)
            {
                inputFile = Directory.GetCurrentDirectory() + "\\" + inputFile;
            }

            if (overlayFile.Contains("\\") == false)
            {
                overlayFile = Directory.GetCurrentDirectory() + "\\" + overlayFile;
            }

            if (outFile.Contains("\\") == false)
            {
                outFile = Directory.GetCurrentDirectory() + "\\" + outFile;
            }
        }
    }
}
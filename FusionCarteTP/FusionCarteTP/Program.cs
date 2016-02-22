using CarteTPService;
using log4net.Config;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace FusionCarteTP
{
    class Program
    {
        static void Main(string[] args)
        {
            //configuration de log4net
            XmlConfigurator.Configure();
            ServiceCfg.CheckConfiguration();

            //on sépare les pages du pdf d'entrée
            //PdfManager.SplitPdf(e.FullPath);

            //on recherche les fichiers(page) séparés 
            var files = PdfManager.FindPdfFiles(ServiceCfg.TempFolder);
            if (files.Any())
            {
                //pour chaque page de superposition
                foreach (var f in files)
                {
                    try
                    {
                        //on effectue la superposition
                        //if (PdfManager.OverlayPdf(f))
                        //{
                            //on extrait le texte du fichier
                            string ptext = PdfManager.GetPdfText(f);
                            //on sectionne par ligne
                            string[] splitedtext = ptext.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                            XmlManager.PrepareData(splitedtext);
                            XmlManager.GenerateXml($"{ServiceCfg.OutputFolderPath}CTP_{Path.GetFileName(f)}");

                            File.Delete(f);
                        //}
                    }
                    catch (Exception x)
                    {
                        ServiceCfg.Log.Error($"Service : Erreur fichier {f} -", x);
                    }
                }
            }


            //var mode = ConfigurationManager.AppSettings["mode"];

            //switch (mode)
            //{
            //    case "split":
            //        SplitDoc();
            //        break;
            //    case "grouped":
            //        GroupedDoc();
            //        break;
            //    case "untouched":
            //        UntouchedDoc();
            //        break;
            //    default:
            //        SplitDoc();
            //        break;
            //}

        }

        void DrawImage(XGraphics gfx, string jpegSamplePath, int x, int y, int width, int height)
        {
            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, x, y, width, height);
        }

        private static void Save(PdfDocument pdf)
        {
            //pdf.Save();
        }

        private static void SplitDoc()
        {
            //fichier model
            var modelfile = ConfigurationManager.AppSettings["modelfile"];
            var filespath = ConfigurationManager.AppSettings["filespath"];

            PdfDocument docbase = new PdfDocument(modelfile);

            //recherche du fichier dans le repertoire donné
            var files = Directory.EnumerateFiles(filespath, "*.pdf");

            if (files.Any())
            {
                foreach (string p in files)
                {

                }

            }

            //// Create an empty page or load existing
            //    PdfPage page = document.AddPage();

            //// Get an XGraphics object for drawing
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //DrawImage(gfx, imageLoc, 50, 50, 250, 250);

            //// Save and start View
            //document.Save(filename);
            //Process.Start(filename);
        }

        private static void GroupedDoc()
        {
            ////fichier model
            //var modelfile = ConfigurationManager.AppSettings["modelfile"];
            //var filespath = ConfigurationManager.AppSettings["filespath"];

            //PdfDocument document = new PdfDocument();

            //// Create an empty page or load existing
            //PdfPage page = document.AddPage();

            //// Get an XGraphics object for drawing
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //DrawImage(gfx, imageLoc, 50, 50, 250, 250);

            //// Save and start View
            //document.Save(filename);
            //Process.Start(filename);
        }

        private static void UntouchedDoc()
        {
            ////fichier model
            //var modelfile = ConfigurationManager.AppSettings["modelfile"];
            //var filespath = ConfigurationManager.AppSettings["filespath"];

            //PdfDocument document = new PdfDocument();

            //// Create an empty page or load existing
            //PdfPage page = document.AddPage();

            //// Get an XGraphics object for drawing
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //DrawImage(gfx, imageLoc, 50, 50, 250, 250);

            //// Save and start View
            //document.Save(filename);
            //Process.Start(filename);
        }

        private static void SplitFile()
        {

            var filespath = ConfigurationManager.AppSettings["modelfile"];
            var finalfolder = ConfigurationManager.AppSettings["finalfolder"];

            //recherche du fichier dans le repertoire donné
            var files = Directory.EnumerateFiles(filespath, "*.pdf");

            if (files.Any())
            {
                foreach (string p in files)
                {
                    var srcDoc = Path.Combine(filespath, Path.GetFileName(p));
                    //var cpyDoc = Path.Combine(finalfolder, Path.GetFileName(p));
                    //File.Copy(srcDoc, cpyDoc, true);

                    // Open the file
                    PdfDocument inputDocument = PdfReader.Open(srcDoc, PdfDocumentOpenMode.Import);

                    string name = Path.GetFileNameWithoutExtension(p);
                    for (int idx = 0; idx < inputDocument.PageCount; idx++)
                    {
                        // Create new document
                        PdfDocument outputDocument = new PdfDocument();
                        outputDocument.Version = inputDocument.Version;
                        outputDocument.Info.Title =
                            string.Format("Page {0} of {1}", idx + 1, inputDocument.Info.Title);
                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        // Add the page and save it
                        outputDocument.AddPage(inputDocument.Pages[idx]);
                        outputDocument.Save(Path.Combine(finalfolder, string.Format("{0} - Page {1}_tempfile.pdf", name, idx + 1)));
                    }
                }
            }
        }
    }
}

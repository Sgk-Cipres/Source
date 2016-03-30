using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;

using PdfSharpReader = PdfSharp.Pdf.IO.PdfReader;
using PdfSharpDocument = PdfSharp.Pdf.PdfDocument;
using PdfSharpPage = PdfSharp.Pdf.PdfPage;
using PdfReader = iTextSharp.text.pdf.PdfReader;
using PdfDocument = iTextSharp.text.pdf.PdfDocument;
using iTextSharpPath = iTextSharp.text.pdf.parser.Path;
using Path = System.IO.Path;


namespace CarteTPExtranet
{
    /// <summary>
    /// on exploite ici 2 librairies pour la gestion de documents PDF "PdfSharp" et "iTextSharp"
    /// elles font dans l'ensemble les même manipulations autour des fichiers PDF
    /// cependant on profite par l'occasion de tester les possibilités de chacune
    /// </summary>
    public static class PdfManager
    {
        private static string _lastPdf;

        public static string LastPdf
        {
            get { return _lastPdf; }
        }

        /// <summary>
        /// vérifie si le répertoire donné existe
        /// (optionnel) créé ce répertoire
        /// </summary>
        /// <param name="path">chemin à vérifier</param>
        /// <param name="cancreate"></param>
        /// <returns></returns>
        public static bool CheckFolder(string path, bool cancreate)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    if (cancreate)
                    {
                        Directory.CreateDirectory(path);
                        return true;
                    }
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                { }
                throw new Exception("PdfManager.CheckFolder : ", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileOne"></param>
        /// <param name="fileTwo"></param>
        /// <param name="newFile"></param>
        public static void ConcatPdf(string fileOne, string fileTwo, string newFile)
        {
            //si le premier fichier en entrée est aussi le fichier de sortie on concatène ce dernier
            var append = fileOne.Equals(newFile);

            if (append)
            {
                //on construit le fichier sortie dans un autre emplacement
                string cardFolder = Path.Combine(ServiceCfg.OutputFolderPath, @"pages\");
                newFile = Path.Combine(cardFolder, Path.GetFileName(newFile));
            }
            //fusion des pdf
            using (FileStream stream = new FileStream(newFile, FileMode.Create))
            {
                Document pdfDoc = new Document();
                PdfCopy pdf = new PdfCopy(pdfDoc, stream);
                pdfDoc.Open();
                if (File.Exists(fileOne))
                    pdf.AddDocument(new PdfReader(fileOne));
                if (File.Exists(fileTwo))
                    pdf.AddDocument(new PdfReader(fileTwo));
                
                if (pdfDoc != null)
                {
                    pdfDoc.Close();
                    if (append)
                        //on déplace le fichier sortie obtenu à l'emplacement d'origine
                        File.Move(newFile, fileOne);
                }
                    
            }
        }

        /// <summary>
        /// permet de séparer les pages du document source 
        /// vers le répertoire temporaire
        /// <param name="file">fichier source</param>
        /// </summary>
        public static void SplitPdf(string file)
        {
            SplitPdf(file, true);
        }

        /// <summary>
        /// permet de séparer les pages du document source 
        /// vers le répertoire temporaire
        /// <param name="file">fichier source</param>
        /// <param name="copy">true:copie false:déplace</param>
        /// </summary>
        public static void SplitPdf(string file, bool copy)
        {
            try
            {
                var srcDoc = file;

                //ouverture du fichier
                PdfSharpDocument inputDocument = PdfSharpReader.Open(srcDoc, PdfDocumentOpenMode.Import);

                string name = Path.GetFileNameWithoutExtension(file);
                if(inputDocument.PageCount>1)
                {
                    for (int idx = 0; idx < inputDocument.PageCount; idx++)
                    {
                        //nouveau document
                        PdfSharpDocument outputDocument = new PdfSharpDocument();
                        outputDocument.Version = inputDocument.Version;
                        outputDocument.Info.Title = $"Page {idx + 1} of {inputDocument.Info.Title}";
                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        //ajout de la page et sauvegarde
                        outputDocument.AddPage(inputDocument.Pages[idx]);
                        CheckFolder(ServiceCfg.TempFolder, true);
                        outputDocument.Save(Path.Combine(ServiceCfg.TempFolder, $"{name}__p{idx + 1}.pdf"));
                        outputDocument.Close();
                    }
                }
                else
                {
                    //nouveau document
                    PdfSharpDocument outputDocument = new PdfSharpDocument();
                    outputDocument.Version = inputDocument.Version;
                    outputDocument.Info.Title = inputDocument.Info.Title;
                    outputDocument.Info.Creator = inputDocument.Info.Creator;

                    //ajout de la page et sauvegarde
                    outputDocument.AddPage(inputDocument.Pages[0]);
                    CheckFolder(ServiceCfg.TempFolder, true);
                    outputDocument.Save(Path.Combine(ServiceCfg.TempFolder, $"{name}.pdf"));
                    outputDocument.Close();
                }

                if (CheckFolder(ServiceCfg.OutputFolderPath,false))
                {
                    var pathbckup = Path.Combine(ServiceCfg.OutputFolderPath, "original\\");
                    var filebck = Path.Combine(pathbckup, Path.GetFileName(file));
                    if (CheckFolder(pathbckup, true))
                    {
                        if(File.Exists(filebck))
                        {
                            filebck = Path.Combine(pathbckup, $"{Path.GetFileNameWithoutExtension(file)}_[ALT-{DateTime.Now.ToString("yyyyMMddHHmmss")}]{Path.GetExtension(file)}");
                        }
                        if(copy)
                            File.Copy(srcDoc, filebck);
                        else
                            File.Move(srcDoc, filebck);
                    }
                }
            }
            catch (Exception e)
            {
                ServiceCfg.Log.Error($"PdfManager.SplitPdf : {file}{Environment.NewLine}", e);
                throw new Exception($"PdfManager.SplitPdf : {file}{Environment.NewLine}", e);
            }
        }

        /// <summary>
        /// réorganise les pages d'un document pdf selon la liste d'indices de pages
        /// <param name="inputFile">fichier à ordonner</param>
        /// <param name="indexList">pages dans l'ordre voulu</param>
        /// </summary>
        public static void DoReorder(string inputFile, int[] indexList)
        {
            //var inputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Test.pdf");
            //var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Output.pdf");
            var output = Path.Combine(ServiceCfg.OutputFolderPath, Path.GetFileName(inputFile));

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
                        copy.ReorderPages(indexList);
                        doc.Close();
                    }
                }
            }
        }

        /// <summary>
        /// déplace un pdf en erreur dans un répertoire dédié
        /// </summary>
        /// <param name="file">fichier à déplacer</param>
        /// <returns>nouvel emplacement</returns>
        public static string MovePdfError(string file)
        {
            if (CheckFolder(ServiceCfg.OutputFolderPath, false))
            {
                var pathbckup = Path.Combine(ServiceCfg.OutputFolderPath, "erreur\\");
                if (CheckFolder(pathbckup, true))
                {
                    if (File.Exists(file))
                    {
                        File.Move(file, Path.Combine(pathbckup, Path.GetFileName(file)));
                        return Path.Combine(pathbckup, Path.GetFileName(file));
                    }
                }
            }
            return file;
        }

        /// <summary>
        /// obtien une collection de fichier pdf trouvés dans un repertoire
        /// </summary>
        /// <param name="path">repertoire pour la recherche</param>
        /// <returns>collection de chemins complet de fichier PDF</returns>
        public static IEnumerable<string> FindPdfFiles(string path)
        {
            return Directory.EnumerateFiles(path, "*.pdf");
        }

        /// <summary>
        /// extrait le contenu texte d'un fichier PDF
        /// </summary>
        /// <param name="pdfpath">chemin complet pour le fichier</param>
        /// <returns>text en brut</returns>
        public static string GetPdfText(string pdfpath)
        {
            //on exploite iTextSharp

            //Creation du reader
            PdfReader reader = new PdfReader(pdfpath);

            //Creation du contenu en sortie
            StringWriter output = new StringWriter();

            //extraction du texte pour chaque page du document
            for (int i = 1; i <= reader.NumberOfPages; i++)
                output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));

            reader.Close();
            return output.ToString();
        }
    }
}

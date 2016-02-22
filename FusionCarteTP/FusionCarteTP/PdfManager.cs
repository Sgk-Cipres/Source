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



namespace CarteTPService
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
        /// permet de séparer les pages des documents du répertoire source 
        /// (alias 'inputfolder' dans fichier de configuration)
        /// vers le répertoire temporaire
        /// </summary>
        public static void SplitPdf(string file)
        {
            try
            {
                if (CheckFolder(ServiceCfg.InputFolderPath,false))
                {

                    //recherche du fichier dans le repertoire donné
                    //var files = Directory.EnumerateFiles(ServiceCfg.InputFolderPath, "*.pdf");
                    //var files = FindPdfFiles(ServiceCfg.InputFolderPath);
                    string p = file;
                    //if (files.Any())
                    //{
                        //foreach (string p in files)
                        //{
                            var srcDoc = Path.Combine(ServiceCfg.InputFolderPath, Path.GetFileName(p));
                            //var cpyDoc = Path.Combine(finalfolder, Path.GetFileName(p));
                            //File.Copy(srcDoc, cpyDoc, true);

                            // Open the file
                            PdfSharpDocument inputDocument = PdfSharpReader.Open(srcDoc, PdfDocumentOpenMode.Import);

                            string name = Path.GetFileNameWithoutExtension(p);
                            for (int idx = 0; idx < inputDocument.PageCount; idx++)
                            {
                                // Create new document
                                PdfSharpDocument outputDocument = new PdfSharpDocument();
                                outputDocument.Version = inputDocument.Version;
                                outputDocument.Info.Title = $"Page {idx + 1} of {inputDocument.Info.Title}";
                                outputDocument.Info.Creator = inputDocument.Info.Creator;

                                // Add the page and save it
                                outputDocument.AddPage(inputDocument.Pages[idx]);
                                CheckFolder(ServiceCfg.TempFolder, true);
                                outputDocument.Save(Path.Combine(ServiceCfg.TempFolder, $"{name}__p{idx + 1}.pdf"));
                                outputDocument.Close();
                            }
                            if (CheckFolder(ServiceCfg.OutputFolderPath,false))
                            {
                                var pathbckup = Path.Combine(ServiceCfg.OutputFolderPath, "original\\");
                                if (CheckFolder(pathbckup, true))
                                {
                                    File.Move(srcDoc, Path.Combine(pathbckup, Path.GetFileName(p)));
                                }
                            }
                        //}
                    //}
                }
                else
                {
                    ServiceCfg.Log.Error($"PdfManager.SplitPdf : Pas de répertoire d'entrée {ServiceCfg.InputFolderPath}");
                }
            }
            catch (Exception e)
            {
                ServiceCfg.Log.Error("PdfManager.SplitPdf : ", e);
            }
        }

        /// <summary>
        /// recouvre le fichier modèle avec le contenu des fichiers d'entrée 
        /// (préalablement séparés en 1 page par fichier)
        /// </summary>
        /// <param name="overlayfile">page ayant les données à superposer</param>
        /// <returns>booléen pour indiquer si l'opération a réussit ou pas</returns>
        public static bool OverlayPdf(string overlayfile)
        {
            var isOk = true;
            try
            {
                string inputFile = ServiceCfg.PdfModel;
                string overlayFile = overlayfile;
                string outFile = Path.Combine(ServiceCfg.OutputFolderPath,"CTP_" + Path.GetFileName(overlayFile));

                //Creation du reader et du document pour lire le document PDF original
                PdfReader reader = new PdfReader(inputFile);
                Document inputDoc = new Document(reader.GetPageSizeWithRotation(1));

                using (FileStream fs = new FileStream(outFile, FileMode.Create))
                {
               
                    //Creation du PDF Writer pour créer le nouveau Document PDF
                    PdfWriter outputWriter = PdfWriter.GetInstance(inputDoc, fs);
                    inputDoc.Open();
                    //Creation du Content Byte pour tamponner le PDF writer
                    PdfContentByte cb1 = outputWriter.DirectContent;

                    //Obtien le document PDF à utiliser comme superposition
                    PdfReader overlayReader = new PdfReader(overlayFile);
                    PdfImportedPage overLay = outputWriter.GetImportedPage(overlayReader, 1);

                    //Obtention de la rotation de page de superposition
                    int overlayRotation = overlayReader.GetPageRotation(1);

                    int n = reader.NumberOfPages;

                    //liste des numéros de pages à marquer dans le modèle
                    List<int> pagesList = GetModelPages2Overlay(n);

                    int i = 1;
                    while (i <= n)
                    {
                        //S'assurer que la taille de la nouvelle page correspond à celle du document d'origine
                        inputDoc.SetPageSize(reader.GetPageSizeWithRotation(i));
                        inputDoc.NewPage();

                        PdfImportedPage page = outputWriter.GetImportedPage(reader, i);
                        
                        int rotation = reader.GetPageRotation(i);

                        //Ajout de la page PDF originale avec la bonne rotation
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

                        //si la page en cours est à marquer
                        if (pagesList.Contains(i))
                        {
                            //Ajout de la superposition avec la bonne rotation
                            if (overlayRotation == 90 || overlayRotation == 270)
                            {
                                cb1.AddTemplate(overLay, 0, -1f, 1f, 0, 0,
                                    reader.GetPageSizeWithRotation(i).Height);
                            }
                            else
                            {
                                //cb1.AddTemplate(overLay, 1f, 0, 0, 1f, 0, 0);
                                cb1.AddTemplate(overLay, float.Parse(ServiceCfg.OverlayX.ToString()), float.Parse(ServiceCfg.OverlayY.ToString()), true);
                            }

                        }
                        //Increment de page
                        i++;
                    }
                    //Fermeture du fichier d'entrée
                    inputDoc.Close();
                    //on garde le fichier de sortie
                    _lastPdf = outFile;
                    //Fermeture du reader pour le fichier de superposition
                    overlayReader.Close();
                }
                
                reader.Close();
            }
            catch (Exception e)
            {
                _lastPdf = string.Empty;
                ServiceCfg.Log.Error("PdfManager.OverlayPdf : ", e);
                isOk = false;
            }
            return isOk;
        }

        /// <summary>
        /// obtien la liste des pages à fusionner dans le modèle
        /// </summary>
        /// <param name="maxpages">nombre de pages max</param>
        /// <returns>liste de numéros de pages</returns>
        public static List<int> GetModelPages2Overlay(int maxpages)
        {
            List<int> pgList = new List<int>();

            //si on a paramétré les pages impactées du modèle
            if (!ServiceCfg.ModelPageNumber.Trim().Equals("0"))
            {
                //on traite la valeur définie
                var pages = ServiceCfg.ModelPageNumber.Trim();

                //on sectionne les pages par le séparateur ","
                var pagesarray = pages.Split(',');

                //pour chaque section
                foreach (var p in pagesarray)
                {
                    //a t on un intervalle
                    if (p.Contains("-"))
                    {
                        //on recupère cet intervalle
                        var gap = p.Split('-');

                        //si cet intervalle est valide (borne inf et borne sup)
                        if (gap.Length == 2)
                        {
                            //bornes
                            var h = int.Parse(gap[0]);
                            var j = int.Parse(gap[1]);

                            //si les bornes sont inversées on les remet dans le bon ordre
                            if (h > j)
                            {
                                var t = h;
                                h = j;
                                j = t;
                            }

                            //on ajoute les pages de l'intervalle (bornes incluses)
                            for (var i = h; i <= j; i++)
                            {
                                if (!pgList.Contains(i) && i <= maxpages)
                                    pgList.Add(i);
                            }
                        }
                    }
                    else
                    {
                        //on ajoute la page
                        var i = int.Parse(p);
                        if (!pgList.Contains(i) && i <= maxpages)
                            pgList.Add(i);
                    }
                }
            }
            else
            {
                //on ajoute toutes les pages
                for (var i = 1; i <= maxpages; i++)
                {
                    pgList.Add(i);
                }
            }

            return pgList;
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

using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteTPLibrary
{
    public static class CarteManager
    {
        public static void Initialize()
        {
            //configuration de log4net
            XmlConfigurator.Configure();
            ServiceCfg.CheckConfiguration();
        }

        public static void DoCards(string lot)
        {
            try {
                PdfManager.SplitPdf(lot);

                //on recherche les fichiers(page) séparés 
                var files = PdfManager.FindPdfFiles(ServiceCfg.TempFolder);
                if (files.Any())
                {
                    //pour chaque page de superposition
                    foreach (var f in files)
                    {
                        try
                        {
                            DataManager.InitDico();
                            //on sectionne le nom pour obtenir le numero de page dans le nom
                            string[] s = Path.GetFileNameWithoutExtension(f).Split('_');
                            //numero de page en fin de nom et commence par "p" (ex p302)
                            if (s[s.Length - 1].StartsWith("p"))
                            {
                                //on sauvegarde le numero de page trouvé
                                DataManager.SetDicoValue(LogTableParam.Page, s[s.Length - 1].Substring(1));
                            }

                            //on extrait le texte du fichier
                            string ptext = PdfManager.GetPdfText(f);
                            //on sectionne par ligne
                            string[] splitedtext = ptext.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                            //on construit les infos Assuré Principal
                            DataManager.PrepareData(splitedtext);
                            if (DataManager.HasChanged)
                                DataManager.GetDataTiers();
                            var filename = DataManager.Enveloppe;
                            //on effectue la superposition
                            if (PdfManager.OverlayPdf(f))
                            {
                                //fichier enveloppe
                                PdfManager.ConcatPdf(filename, PdfManager.LastPdf, filename);

                                //on actualise l'enveloppe dans le dico
                                DataManager.SetDicoValue(LogTableParam.Enveloppe, filename);

                                //fichier xml
                                XmlManager.GenerateXml();

                                //à ce stade tout est OK on log le traitement réussit
                                DataManager.SetLogTable(1, $"Traitement cartes {DataManager.GetLastCardsSerial()} terminé avec succès");

                                File.Delete(f);
                            }
                            else
                            {
                                DataManager.SetDicoValue(LogTableParam.Intermediaire, PdfManager.MovePdfError(f));
                                var msg = DataManager.GetLastCardsSerial().Contains(",") ? "des Cartes " : "de la Carte";
                                DataManager.SetLogTable(-1, $"Generation {msg} {DataManager.GetLastCardsSerial()} a échouée");
                            }
                        }
                        catch (Exception x)
                        {
                            ServiceCfg.Log.Error($"Service : Erreur fichier {Path.GetFileName(f)} -", x);
                            if (!string.IsNullOrEmpty(DataManager.Enveloppe) && File.Exists(DataManager.Enveloppe))
                                DataManager.SetDicoValue(LogTableParam.Enveloppe, PdfManager.MovePdfError(DataManager.Enveloppe));
                            DataManager.SetDicoValue(LogTableParam.Intermediaire, PdfManager.MovePdfError(f));

                            DataManager.SetLogTable(-1, $"Service : Erreur fichier {Path.GetFileName(f)} -" + x.GetBaseException().Message);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ServiceCfg.Log.Error($"Service : Erreur fichier {lot} -", e);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteTPService
{
    public class Worker
    {
        public void DoWork(string file)
        {
            //on sépare les pages du pdf d'entrée
            PdfManager.SplitPdf(file);

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
                        if (PdfManager.OverlayPdf(f))
                        {
                            //on extrait le texte du fichier
                            string ptext = PdfManager.GetPdfText(f);
                            //on sectionne par ligne
                            string[] splitedtext = ptext.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                            DataManager.PrepareData(splitedtext);
                            XmlManager.GenerateXml();

                            File.Delete(f);
                        }
                    }
                    catch (Exception x)
                    {
                        ServiceCfg.Log.Error($"Erreur fichier {f} -", x);
                    }
                }
            }
        }
    }
}

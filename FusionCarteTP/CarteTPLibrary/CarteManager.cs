using log4net.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
                var files = PdfManager.FindPdfFiles(Path.Combine(ServiceCfg.TempFolder,Path.GetFileNameWithoutExtension(lot)));
                if (files.Any())
                {
                    //pour chaque page de superposition
                    foreach (var f in files)
                    {
                        try
                        {
                            //on extrait le texte du fichier
                            string ptext = PdfManager.GetPdfText(f);
                            //on sectionne par ligne
                            string[] splitedtext = ptext.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                            //on construit les infos Assuré Principal
                            DataManager.PrepareData(splitedtext);

                            if (!string.IsNullOrEmpty(DataManager.AssureP))
                            {

                                //on sectionne le nom pour obtenir le numero de page dans le nom
                                string[] s = Path.GetFileNameWithoutExtension(f).Split('_');
                                //numero de page en fin de nom et commence par "p" (ex p302)
                                if (s[s.Length - 1].StartsWith("p"))
                                {
                                    //on sauvegarde le numero de page trouvé
                                    DataManager.SetDicoValue(LogTableParam.Page, s[s.Length - 1].Substring(1));
                                }

                                var filename = DataManager.Enveloppe;

                                //if (DataManager.HasChanged)
                                //{
                                //    DataManager.GetDataTiers();
                                //}

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
                            else
                            {
                                PdfManager.MovePdfError(DataManager.Enveloppe);
                                PdfManager.MovePdfError(f);
                            }
                        }
                        catch (Exception x)
                        {
                            ServiceCfg.Log.Error($"Service : Erreur fichier {Path.GetFileName(f)} -", x);
                            if (!string.IsNullOrEmpty(DataManager.Enveloppe) && File.Exists(DataManager.Enveloppe))
                                DataManager.SetDicoValue(LogTableParam.Enveloppe, PdfManager.MovePdfError(DataManager.Enveloppe));
                            DataManager.SetDicoValue(LogTableParam.Intermediaire, PdfManager.MovePdfError(f));

                            DataManager.SetLogTable(-1, $"Service : Erreur fichier page {Path.GetFileName(f)} -" + x.GetBaseException().Message);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ServiceCfg.Log.Error($"Service : Erreur fichier lot {lot} -", e);
            }

        }

        public static void DoCardsFromDB()
        {
            var cs = ServiceCfg.ConnectionString;
            var f = new MemoryStream();
            using(SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlTransaction txn = con.BeginTransaction();
                
                SqlCommand cmd = new SqlCommand("ListerPdfLot", con, txn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    
                    string filePath = rdr[0].ToString();
                    byte[] objContext = (byte[])rdr[1];
                    string fName = rdr[2].ToString();

                    f = new MemoryStream(objContext);

                    PdfManager.OverlayPdf(f, fName);

                    ////SqlFileStream sfs = new SqlFileStream(filePath, objContext, System.IO.FileAccess.Read);

                    ////byte[] buffer = new byte[(int)sfs.Length];
                    ////sfs.Read(buffer, 0, buffer.Length);
                    ////sfs.Close();

                    // Just write all files in the table to the temp direcotory.
                    // This is probably not how you would do it in the real world. But this is just an example.
                    string filename = @"C:\Temp\" +fName;

                    //System.IO.FileStream fs = new System.IO.FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write);
                    //fs.Write(buffer, 0, buffer.Length);
                    //fs.Flush();
                    //fs.Close();
                }

                rdr.Close();
                txn.Commit();
                con.Close();
            }
            //if (PdfManager.OverlayPdf(f))
            //{

            //}

        }
    }
}

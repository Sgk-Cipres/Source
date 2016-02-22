using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CarteTPService
{
    class XmlManager
    {
        public static string Nir13;
        public static string Cle;
        public static string DateEdition;
        public static string Edition;

        /// <summary>
        /// recherche des données exploitables dans un tableau de texte
        /// </summary>
        /// <param name="dataStrings"></param>
        public static void PrepareData(string[] dataStrings)
        {
            Nir13 = string.Empty;
            Cle = string.Empty;
            DateEdition = string.Empty;
            Edition = string.Empty;

            foreach (var s in dataStrings)
            {
                if (string.IsNullOrEmpty(Nir13) || string.IsNullOrEmpty(DateEdition) || string.IsNullOrEmpty(Edition))
                {
                    //on cherche un contenu ayant la date d'edition du document
                    if (s.Contains("Edité") && (string.IsNullOrEmpty(DateEdition) || string.IsNullOrEmpty(Edition)))
                    {
                        Edition = s;
                        foreach (var part in s.Split(' '))
                        {
                            DateTime d = DateTime.Now;
                            if (DateTime.TryParseExact(part, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out d))
                            {
                                DateEdition =
                                    DateTime.ParseExact(part, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                                        .ToShortDateString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        //si on a pas de Nir13
                        if (string.IsNullOrEmpty(Nir13))
                        {
                            //ici on va récupéré le numero INSEE de l'adhérent 
                            //qui doit faire partie des informations contenues dans le document
                            //(information qui doit etre contenue dans une ligne et isolée)
                            var num = string.Empty;

                            //on match les chaines de 13 caractères maximum (NIR sans clé)
                            if (s.Replace(" ", "").Length == 13)
                            {
                                //on extrait de la chaine un potentiel NIR
                                if (!string.IsNullOrEmpty(NumeroINSEE.MatchNumeroInsee(s)))
                                {
                                    num = NumeroINSEE.NettoyerNumero(NumeroINSEE.MatchNumeroInsee(s));
                                    Cle = NumeroINSEE.CalculerCleINSEE(num).ToString("D2");
                                    if (NumeroINSEE.VerifierINSEE(num + Cle))
                                    {
                                        //un NIR est trouvé on le garde
                                        Nir13 = num;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            //si à la fin de la recherche on a rien obtenu on log en erreur
            if (string.IsNullOrEmpty(Nir13))
                throw new Exception("XmlManager.PrepareData : Aucune information exploitable dans le document");
        }

        public static void GenerateXml()
        {
            GetData4Xml();
        }

        public static void CreateXml(SqlDataReader data)
        {
            try
            {
                //création du doc xml vierge
                XmlDocument doc = new XmlDocument();
                //entete xml
                XmlNode entete = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(entete);
                //noeud racine
                XmlElement tagDoc = doc.CreateElement("document");
                //noeud chemin fichier
                XmlElement tagFile = doc.CreateElement("file_path");
                tagFile.InnerText = PdfManager.LastPdf;
                tagDoc.AppendChild(tagFile);
                //noued nom de fichier
                XmlElement tagFilename = doc.CreateElement("localname");
                tagFilename.InnerText = Path.GetFileName(PdfManager.LastPdf);
                tagDoc.AppendChild(tagFilename);
                //noeud date de d'édition
                XmlElement tagFileDate = doc.CreateElement("date_doc_creation");
                tagFileDate.InnerText = DateEdition;
                tagDoc.AppendChild(tagFileDate);
                //infos en commentaire
                XmlElement tagComm = doc.CreateElement("commentaire");
                tagComm.InnerText = Edition;
                tagDoc.AppendChild(tagComm);
                //noeuds données sql
                data.Read();
                for (int i = 0; i < data.FieldCount; i++)
                {
                    XmlElement tag = doc.CreateElement(data.GetName(i));
                    tag.InnerText = data.GetValue(i).ToString();
                    tagDoc.AppendChild(tag);
                }
                doc.AppendChild(tagDoc);

                //emplacement et nom de fichier à créer
                string flow = string.Empty;
                if (data["thereflow"] != DBNull.Value)
                    flow = data["thereflow"].ToString();

                string filename = $"{flow}_{data["type_tiers"]}_{Path.GetFileName(PdfManager.LastPdf)}.xml";

                if (File.Exists(Path.Combine(ServiceCfg.OutputFolderPath, filename)))
                    File.Delete(Path.Combine(ServiceCfg.OutputFolderPath, filename));
                doc.Save(Path.Combine(ServiceCfg.OutputFolderPath, filename));
            }
            catch (Exception ex)
            {
                ServiceCfg.Log.Error("XmlManager.GenerateXML : ", ex);
            }
        }

        private static void GetData4Xml()
        {
            //connexion à la base
            using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand("GetTierDataByInsee", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@insee", Nir13 + Cle));

                try
                {
                    //execution de la commande
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //si un résultat existe
                        if (reader.HasRows)
                        {
                            CreateXml(reader);
                        }
                        else
                        {
                           throw new Exception($"Aucune donnée trouvée en base concernant '{Nir13 + Cle}' voir fichier '{PdfManager.LastPdf}'");
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceCfg.Log.Error("XmlManager.GetData4Xml : ", e);
                }
            }
        }
    }
}

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

namespace CarteTPLibrary
{
    public static class XmlManager
    {

        public static void GenerateXml()
        {
            DataManager.GetData4Xml();
        }

        public static void CreateXml(SqlDataReader data)
        {
            try
            {
                var env = DataManager.Enveloppe;
                //création du doc xml vierge
                XmlDocument doc = new XmlDocument();
                //entete xml
                XmlNode entete = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(entete);
                //noeud racine
                XmlElement tagDoc = doc.CreateElement("document");
                //noeud chemin fichier
                XmlElement tagFile = doc.CreateElement("file_path");
                tagFile.InnerText = env;
                tagDoc.AppendChild(tagFile);
                //noued nom de fichier
                XmlElement tagFilename = doc.CreateElement("localname");
                tagFilename.InnerText = Path.GetFileName(env);
                tagDoc.AppendChild(tagFilename);
                //noeud date de d'édition
                XmlElement tagFileDate = doc.CreateElement("date_doc_creation");
                tagFileDate.InnerText = DataManager.DateEdition;
                tagDoc.AppendChild(tagFileDate);
                //infos en commentaire
                XmlElement tagComm = doc.CreateElement("commentaire");
                tagComm.InnerText = DataManager.Edition;
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
 
                string filename = $"{data["thereflow"]}_{data["type_tiers"]}_{Path.GetFileNameWithoutExtension(env)}.xml";
                string xml = Path.Combine(ServiceCfg.OutputFolderPath, DateTime.Parse(DataManager.DateEdition).ToString("yyyyMMdd"), filename);
                if (File.Exists(xml))
                    File.Delete(xml);
                doc.Save(xml);

                DataManager.SetDicoValue(LogTableParam.Xml, xml);
            }
            catch (Exception ex)
            {
                ServiceCfg.Log.Error("XmlManager.GenerateXML : ", ex);
                DataManager.SetLogTable(-1, "XmlManager.GenerateXML : " + ex.Message);
            }
        }

    }
}

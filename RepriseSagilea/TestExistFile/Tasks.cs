using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using log4net;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;

namespace TestExistFile
{
    public static class Tasks
    {
        private static string basePath = ConfigurationManager.AppSettings["basepath"];
        private static string depot = ConfigurationManager.AppSettings["depotxml"];
        private static string connectionString = ConfigurationManager.AppSettings["csSagilea"];
        private static string connectionStringTherefore = ConfigurationManager.AppSettings["csTherefore"];
        private static string mode = ConfigurationManager.AppSettings["mode"];
        private static readonly ILog Log = LogManager.GetLogger("logging");
        private static TasksStatus taskProgress = new TasksStatus();

        public static void ExecuteMainTask(string[] args)
        {
            string debut = string.Empty;
            string fin = string.Empty;
            //on teste les arguments commande
            if (args.Length == 2)//bon nombre d'arguments attendus
            {
                try
                {
                    //valeurs convertibles en date valides et l'ordre de ces dates
                    if(DateTime.ParseExact(args[0], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) 
                        <= DateTime.ParseExact(args[1], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        debut = args[0];
                        fin = args[1];
                    }
                    else
                    {
                        debut = args[1];
                        fin = args[0];
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Ligne de commande : Arguments non valides", e);
                    Environment.Exit(-1);
                }
            }
            else
            {
                Log.Error("Ligne de commande : Arguments date debut et date fin absents (Format AAAAMMJJ)");
                Environment.Exit(-1);
            }
            //connexion à la base
            using (SqlConnection connection = new SqlConnection(connectionStringTherefore))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand("GetRepriseData", connection);
                //délai d'attente requête 10 minutes max
                command.CommandTimeout = 600;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@mode", mode));
                command.Parameters.Add(new SqlParameter("@debut", debut));
                command.Parameters.Add(new SqlParameter("@fin", fin));

                //compteurs
                int cpt = 0;
                int row = 0;

                try
                {
                    bool isStopWatch = bool.Parse(ConfigurationManager.AppSettings["watchdetail"]);

                    //chrono temps d'execution
                    Stopwatch watch = new Stopwatch();
                    if (isStopWatch)
                        watch = Stopwatch.StartNew();

                    //execution de la commande
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (isStopWatch)
                        { 
                            watch.Stop();

                            Log.Info(string.Format("Temps d'execution requête sql : {0} ms (soit {1})"
                                , watch.ElapsedMilliseconds.ToString()
                                , GetTimeFromMilliseconds(watch.ElapsedMilliseconds)));
                        }

                        //si un résultat existe
                        if (reader.HasRows)
                        {
                            List<string> lst = new List<string>();
                            lst.Add("CheckFileTask");
                            lst.Add("RefValidationTask");
                            lst.Add("GenerateXmlTask");
                            lst.Add("DecryptPdfTask");

                            //lecture de chaque ligne obtenue
                            while (reader.Read())
                            {
                                taskProgress.ResetList(lst);

                                watch = Stopwatch.StartNew();
                                //incrémentation du compteur de lignes
                                row++;
                                try
                                {
                                    //si un identifiant de pièce jointe existe 
                                    if (reader["no_doc"] != System.DBNull.Value)
                                    {
                                        //nom du fichier
                                        string filename = reader["no_doc"] as string;
                                        //arborescence liée au fichier
                                        string partial = GetPartialPath(filename);
                                        string media = reader["media_lib"] as string;
                                        //chemin complet
                                        string fullPath = media.Equals("Téléphone") || media.Equals("Interne") ? depot + "FichesPDF\\" : basePath + partial + "\\";
                                        //recherche du fichier dans le repertoire donné
                                        var files = Directory.EnumerateFiles(fullPath, "*" + filename + ".*");
                                        //si un fichier correspond
                                        if (files.Count() > 0)
                                        {
                                            taskProgress.SetTaskStatus("CheckFileTask", Status.OK);
                                            //flag si doc crypté
                                            bool isCrypted = false;
                                            //on garde en priorité les fichiers PDF
                                            string docpath = string.Empty;
                                            foreach (string p in files)
                                            {
                                                string ext = Path.GetExtension(p);
                                                if (ext.ToLower().Equals(".pdf"))
                                                {
                                                    docpath = p;
                                                    break;
                                                }
                                                else
                                                {
                                                    if (ext.Equals(".crypt"))
                                                        isCrypted = true;
                                                    //!!peut etre non necessaire dépend de l'emplacement des fichiers XML
                                                    if (!ext.ToLower().Equals(".xml"))
                                                        docpath = p;
                                                }
                                            }

                                            //on décrypte le doc
                                            if (isCrypted)
                                                docpath = DecryptPDF(docpath);
                                            else
                                                taskProgress.SetTaskStatus("DecryptPdfTask", Status.NO);

                                            if (docpath.Equals(string.Empty))
                                                throw new Exception("Perte de la définition du répertoire source");

                                            //on constitue le fichier XML de reprise
                                            GenerateXML(reader, docpath);
                                            //test
                                            //taskProgress.SetTaskStatus("GenerateXmlTask", Status.OK);
                                        }
                                        else
                                        {
                                            //incrementation du compteur de lignes en erreur
                                            cpt++;
                                            string msg = string.Format("Ligne {0} : Fichier {1} introuvable {2}- event : id-{3} type-{4} media-{5} sens-{6}"
                                                , row, fullPath + filename, Environment.NewLine + "\t"
                                                , reader["evt_id"].ToString(), reader["evt_lib"].ToString()
                                                , reader["media_lib"].ToString(), reader["sens"].ToString());
                                            taskProgress.SetTaskStatus("CheckFileTask", Status.KO);

                                            taskProgress.CurrentTaskMessage = msg;
                                            Log.Error(msg);
                                        }

                                    }
                                }
                                catch (Exception x)
                                {
                                    taskProgress.SetTaskStatus("CheckFileTask", Status.KO);
                                    taskProgress.CurrentTaskMessage = string.Format("Recherche Fichier : {0}", x.Message);
                                    Log.Error("Recherche Fichier", x);
                                }

                                //arret chrono
                                watch.Stop();
                                taskProgress.CurrentTaskDuration = watch.ElapsedMilliseconds.ToString();

                                if (isStopWatch)
                                {
                                    Log.Info(string.Format("ligne {0} traitée en {1} ms"
                                        , row, watch.ElapsedMilliseconds.ToString()));
                                }

                                int etat = 0;
                                if(taskProgress.GetTaskStatus("GenerateXmlTask") == Status.OK)
                                {
                                    taskProgress.CurrentTaskMessage = "Traitement éffectué avec succès";
                                    etat = 1;
                                }
                                else
                                {
                                    if(taskProgress.GetTaskStatus(taskProgress.CurrentTask) == Status.KO)
                                    {
                                        etat = -1;
                                    }
                                }

                                UpdateFlag(int.Parse(reader["evt_id"].ToString()), etat);
                            }

                        }
                    }
                    //commande pour obtenir le total de lignes
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select @@ROWCOUNT";
                    var totalRows = command.ExecuteScalar();
                    Log.Info(string.Format("Total de lignes lues : {0} dont {1} en erreur"
                        , totalRows.ToString(), cpt));
                }
                catch (Exception e)
                {
                    taskProgress.CurrentTaskMessage = string.Format("Exception base de données : {0}", e.Message);
                    Log.Error("Exception base de données", e);
                }
                //clôture
                connection.Close();
            }

        }

        /// <summary>
        /// Constitue un fichier XML avec les informations fournies pour un document GED
        /// </summary>
        /// <param name="data">données associées au document GED</param>
        /// <param name="docpath">emplacement du document GED</param>
        public static void GenerateXML(SqlDataReader data, string docpath)
        {
            try
            {
                //création du doc xml vierge
                XmlDocument doc = new XmlDocument();
                //entete xml
                XmlNode entete = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(entete);
                //noeud racine
                XmlElement tag_doc = doc.CreateElement("document");
                //noeud chemin fichier GED
                XmlElement tag_file = doc.CreateElement("file_path");
                tag_file.InnerText = docpath;
                tag_doc.AppendChild(tag_file);
                //noeud date de numérisation
                XmlElement tag_file_date = doc.CreateElement("date_doc_creation");
                var s = data["no_doc"].ToString().Split('_');
                DateTime date = DateTime.ParseExact(s[1], "yyyyMMddHHmmssFFF", System.Globalization.CultureInfo.InvariantCulture);
                tag_file_date.InnerText = date.ToString("dd/MM/yyyy HH:mm:ss");
                tag_doc.AppendChild(tag_file_date);
                //noeuds données sql
                for (int i = 0; i < data.FieldCount; i++)
                {
                    XmlElement tag = doc.CreateElement(data.GetName(i));
                    tag.InnerText = data.GetValue(i).ToString();
                    tag_doc.AppendChild(tag);
                }
                doc.AppendChild(tag_doc);

                //emplacement et nom de fichier à créer
                string flow = string.Empty;
                if (data["thereflow"] != DBNull.Value)
                    flow = data["thereflow"].ToString();

                string filename = flow + "_" + data["type_tiers"].ToString() + "_" + data["no_doc"].ToString() + ".xml";
                if (File.Exists(Path.Combine(depot, filename)))
                    File.Delete(Path.Combine(depot, filename));
                doc.Save(Path.Combine(depot, filename));

                taskProgress.SetTaskStatus("GenerateXmlTask", Status.OK);

            }
            catch (Exception ex)
            {
                taskProgress.SetTaskStatus("GenerateXmlTask", Status.KO);
                taskProgress.CurrentTaskMessage = string.Format("Generation fichier Xml : {0}", ex.Message);
                Log.Error("Generation XML", ex);
            }
        }

        /// <summary>
        /// Décrypte un document GED crypté (.crypt)
        /// </summary>
        /// <param name="filepath">emplacement du document GED</param>
        /// <returns>fichier décrypté</returns>
        public static string DecryptPDF(string filepath)
        {
            try
            {
                //emplacement de depot du document décrypté
                string dest = Path.GetDirectoryName(depot + "DocDecrypte\\");
                if (!Directory.Exists(dest))  // si ça n'existe pas on le créé
                    Directory.CreateDirectory(dest);

                //obtention des paramètres de décryptage
                string k = ConfigurationManager.AppSettings["b64key"];
                string v = ConfigurationManager.AppSettings["b64IV"];
                CryptoManager manager = new CryptoManager(filepath, k, v, dest);

                //décryptage
                if(manager.ProcessFileDecryption(filepath))
                {
                    taskProgress.SetTaskStatus("DecryptPdfTask", Status.OK);
                }
                else
                {
                    taskProgress.SetTaskStatus("DecryptPdfTask", Status.KO);
                }

                //renvoi emplacement du fichier fraichement décrypté
                return manager.NewFile;
            }
            catch (Exception ex)
            {
                taskProgress.SetTaskStatus("DecryptPdfTask", Status.KO);
                taskProgress.CurrentTaskMessage = string.Format("Décryptage de document : {0}", ex.Message);
                Log.Error("Décryptage de document", ex);
            }
            return string.Empty;
        }

        public static void ReferenceValidation()
        {
            taskProgress.SetTaskStatus("RefValidationTask", Status.OK);
        }

        private static void UpdateFlag(int id, int state)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionStringTherefore))
                {
                    //ouverture
                    connection.Open();

                    //commande sql
                    SqlCommand command = new SqlCommand("EditMigrationProgress",connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@evenement",id));
                    command.Parameters.Add(new SqlParameter("@etat", state));
                    command.Parameters.Add(new SqlParameter("@etape", taskProgress.CurrentTask));
                    command.Parameters.Add(new SqlParameter("@duree", GetTimeFromMilliseconds(long.Parse(taskProgress.CurrentTaskDuration))));
                    command.Parameters.Add(new SqlParameter("@message", taskProgress.CurrentTaskMessage));

                    if (command.ExecuteNonQuery() == 0)
                    {
                        connection.Close();
                        throw new Exception(string.Format("La mise à jour n'a pas impacté de lignes : evenementiel {0}", id));
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Mise à jour avancement : ", ex);
            }
        }

        /// <summary>
        /// Obtien une partie du repertoire GED à partir du nom du fichier référence 
        /// </summary>
        /// <param name="source">nom du fichier</param>
        /// <returns>repertoire</returns>
        private static string GetPartialPath(string source)
        {
            string[] lst = source.Split('_');

            return lst[1].Substring(0, 4) + "\\" + lst[1].Substring(4, 2) + "\\" + lst[1].Substring(6, 2);
        }

        public static string GetTimeFromMilliseconds(long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);

            string time = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
            return time;
        }
    }
}

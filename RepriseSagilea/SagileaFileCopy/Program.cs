using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace SagileaFileCopy
{
    class Program
    {
        private static string basePath = ConfigurationManager.AppSettings["basepath"];
        private static string depot = ConfigurationManager.AppSettings["depot"];
        private static string connectionString = ConfigurationManager.AppSettings["csSagilea"];
        private static string connectionStringTherefore = ConfigurationManager.AppSettings["csTherefore"];
        private static readonly ILog Log = LogManager.GetLogger("logging");

        static void Main(string[] args)
        {
            //configuration de log4net
            XmlConfigurator.Configure();

            Log.Info("Début du traitement ...");

            bool isStopWatch = bool.Parse(ConfigurationManager.AppSettings["watch"]);

            //chrono temps d'execution
            Stopwatch watch = new Stopwatch();
            if (isStopWatch)
                watch = Stopwatch.StartNew();

            DoJob(args);

            if (isStopWatch)
            {
                //arrêt chrono
                watch.Stop();

                Log.Info(string.Format("Temps d'execution total : {0} ms (soit {1})"
                    , watch.ElapsedMilliseconds.ToString()
                    , GetTimeFromMilliseconds(watch.ElapsedMilliseconds)));
            }
        }

        public static void DoJob(string[] args)
        {
            string debut = string.Empty;
            string fin = string.Empty;
            //on teste les arguments commande
            if (args.Length == 2)//bon nombre d'arguments attendus
            {
                try
                {
                    //valeurs convertibles en date valides et l'ordre de ces dates
                    if (DateTime.ParseExact(args[0], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
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
                SqlCommand command = new SqlCommand("GetFileList", connection);
                //délai d'attente requête 1 minute max
                command.CommandTimeout = 60;
                command.CommandType = CommandType.StoredProcedure;

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

                            //lecture de chaque ligne obtenue
                            while (reader.Read())
                            {
                                watch = Stopwatch.StartNew();
                                //incrémentation du compteur de lignes
                                row++;
                                try
                                {
                                    //si un identifiant de pièce jointe existe 
                                    if (reader["id_pj"] != System.DBNull.Value)
                                    {
                                        //nom du fichier
                                        string filename = reader["id_pj"] as string;
                                        //arborescence liée au fichier
                                        string partial = GetPartialPath(filename);

                                        //chemin complet
                                        string fullPath = basePath + partial + "\\";
                                        //recherche du fichier dans le repertoire donné
                                        var files = Directory.EnumerateFiles(fullPath, "*" + filename + ".*");
                                        //si un fichier correspond
                                        if (files.Count() > 0)
                                        {
                                            string docpath = string.Empty;
                                            foreach (string p in files)
                                            {
                                                string ext = Path.GetExtension(p);

                                                docpath = p;
                                                if (docpath.Equals(string.Empty))
                                                    throw new Exception("Perte de la définition du répertoire source");

                                                File.Copy(docpath, Path.Combine(depot, filename + ext));
                                            }                                       
                                        }
                                        else
                                        {
                                            //incrementation du compteur de lignes en erreur
                                            cpt++;
                                            string msg = string.Format("Ligne {0} : Fichier {1} introuvable {2}- event : id-{3} type-{4} media-{5} sens-{6}"
                                                , row, fullPath + filename, Environment.NewLine + "\t"
                                                , reader["evt_id"].ToString(), reader["evt_lib"].ToString());

                                            Log.Error(msg);
                                        }
                                    }
                                }
                                catch (Exception x)
                                {
                                    Log.Error("Recherche Fichier", x);
                                }

                                //arret chrono
                                watch.Stop();

                                if (isStopWatch)
                                {
                                    Log.Info(string.Format("ligne {0} traitée en {1} ms"
                                        , row, watch.ElapsedMilliseconds.ToString()));
                                }
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
                    Log.Error("Exception base de données", e);
                }
                //clôture
                connection.Close();
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

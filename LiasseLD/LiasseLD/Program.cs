using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiasseLD
{
    class Program
    {
        private static string basePath = ConfigurationManager.AppSettings["basepath"];
        private static string depot = ConfigurationManager.AppSettings["depot"];
        private static string connectionString = ConfigurationManager.AppSettings["cs"];
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

                Log.Info($"Temps d'execution total : {watch.ElapsedMilliseconds} ms (soit {GetTimeFromMilliseconds(watch.ElapsedMilliseconds)})");
            }
        }

        public static void DoJob(string[] args)
        {
            string debut = string.Empty;
            string fin = string.Empty;
            //on teste les arguments commande

            //connexion à la base
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand
                {
                    CommandText = "sp_SuiviImp_GetLiasseLD",
                    CommandTimeout = 60,
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };

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

                            Log.Info($"Temps d'execution requête sql : {watch.ElapsedMilliseconds} ms (soit {GetTimeFromMilliseconds(watch.ElapsedMilliseconds)})");
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
                                    if (reader["N° evènement"] != System.DBNull.Value)
                                    {
                                        //arborescence liée au fichier
                                        string partial = GetPartialPath(reader["Date impression"] as string);

                                        //chemin complet
                                        string fullPath = basePath + partial + "\\";
                                        //nom du fichier
                                        string p = $"{fullPath}{reader["N° evènement"]}.pdf";
                                        //si un fichier existe
                                        if (File.Exists(p))
                                        {
                                            string docpath = string.Empty;
                                            
                                            string ext = Path.GetExtension(p);

                                            docpath = p;
                                            if (docpath.Equals(string.Empty))
                                                throw new Exception("Perte de la définition du répertoire source");

                                            File.Copy(docpath, Path.Combine(depot, $"{reader["Code Courtier"]}-{reader["N° evènement"]}-{reader["Nom assuré"]}{ext}"),true);
                                            
                                        }
                                        else
                                        {
                                            //incrementation du compteur de lignes en erreur
                                            cpt++;
                                            string msg = $"Ligne {row} : Fichier {p} introuvable {Environment.NewLine + "\t"}- event : id-{reader["N° evènement"]} contrat-{reader["N° Contrat"]}";

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
                                    Log.Info($"ligne {row} traitée en {watch.ElapsedMilliseconds} ms");
                                }
                            }
                        }
                    }
                    //commande pour obtenir le total de lignes
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select @@ROWCOUNT";
                    var totalRows = command.ExecuteScalar();
                    Log.Info($"Total de lignes lues : {totalRows} dont {cpt} en erreur");
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
        /// Obtien une partie du repertoire à partir de la date d'impression au format YYYYMMDD
        /// </summary>
        /// <param name="source">date d'impression (YYYYMMDD)</param>
        /// <returns>repertoire</returns>
        private static string GetPartialPath(string source)
        {
            return
                $"{source.Substring(0, 4)}\\{source.Substring(4, 2).TrimStart('0')}\\{source.Substring(6, 2).TrimStart('0')}";
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

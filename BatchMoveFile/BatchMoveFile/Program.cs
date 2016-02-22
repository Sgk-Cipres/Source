using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BatchMoveFile
{
    class Program
    {
        private static string basePath = ConfigurationManager.AppSettings["basepath"];
        private static string depot = ConfigurationManager.AppSettings["depot"];
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

            DoJob();

            if (isStopWatch)
            {
                //arrêt chrono
                watch.Stop();

                Log.Info(string.Format("Temps d'execution total : {0} ms", watch.ElapsedMilliseconds.ToString()));
            }
        }

        public static void DoJob()
        {
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

                watch = Stopwatch.StartNew();
                //incrémentation du compteur de lignes
                row++;

                try
                {
                    //chemin complet
                    string fullPath = basePath;

                    //recherche du fichier dans le repertoire donné
                    var files = Directory.EnumerateFiles(fullPath, "*.xml");
                    //si un fichier correspond
                    if (files.Count() > 0)
                    {

                        string docpath = string.Empty;
                        foreach (string p in files)
                        {
                            //incrementation du compteur de lignes en erreur
                            cpt++;

                            string ext = Path.GetExtension(p);
                            string filename = Path.GetFileName(p);

                            docpath = p;
                            if (docpath.Equals(string.Empty))
                                throw new Exception("Perte de la définition du répertoire source");

                            //traitement xml
                            //création du doc xml vierge
                            XmlDocument doc = new XmlDocument();
                            doc.Load(docpath);
                            var xpathnode = ConfigurationManager.AppSettings["nodexml"];
                            var node = doc.SelectSingleNode(xpathnode);
                            docpath = node.InnerText;
                            filename = Path.GetFileName(docpath);

                            if (ConfigurationManager.AppSettings["mode"].Equals("copy"))
                                File.Copy(docpath, Path.Combine(depot, filename));
                            else
                                File.Move(docpath, Path.Combine(depot, filename));
                        }
                    }
                    else
                    {
                        
                        string msg = string.Format("Ligne {0} : Fichier introuvable", cpt);

                        Log.Error(msg);
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

                var totalRows = 10;
                Log.Info(string.Format("Total de lignes lues : {0} dont {1} en erreur"
                    , totalRows.ToString(), cpt));
            }
            catch (Exception e)
            {
                Log.Error("Exception", e);

                Environment.Exit(-1);
            }

        }
        
    }
}

using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportTelephonique
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("logging");

        static void Main(string[] args)
        {
            //configuration de log4net
            XmlConfigurator.Configure();

            bool isStopWatch = bool.Parse(ConfigurationManager.AppSettings["watchdetail"]);
            Log.Info("Début du traitement ...");

            //chrono temps d'execution
            Stopwatch watch = new Stopwatch();
            if (isStopWatch)
                watch = Stopwatch.StartNew();

            //execution de la tâche
            ReportingManager.ListEvents(args);

            if (isStopWatch)
            {
                //arrêt chrono
                watch.Stop();

                Log.Info(string.Format("Temps d'execution total : {0} ms"
                    , watch.ElapsedMilliseconds.ToString()));
            }
        }
    }
}

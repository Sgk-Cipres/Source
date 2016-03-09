using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarteTPService
{
    public partial class CarteTPService : ServiceBase
    {
        private static FileSystemWatcher watcher = null;
        public CarteTPService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //configuration de log4net
            //XmlConfigurator.Configure();
            CarteTPLibrary.CarteManager.Initialize();

            CarteTPLibrary.ServiceCfg.Log.Info("Démarrage du service ...");
            //ServiceCfg.CheckConfiguration();

            // création de l'observateur sur le répertoire d'entrée
            // et filtre sur les fichiers de type pdf
            FileSystemWatcher watcher = new FileSystemWatcher(CarteTPLibrary.ServiceCfg.InputFolderPath, "*.pdf");

            //Observation des changements sur la date de création, dernière écriture, 
            //renommage de fichiers ou dossiers.
            watcher.NotifyFilter = NotifyFilters.CreationTime
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // ajout des event handlers.
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Demarre l'observation.
            watcher.EnableRaisingEvents = true;
            CarteTPLibrary.ServiceCfg.Log.Info("Service Démarré");
        }

        protected override void OnStop()
        {
            CarteTPLibrary.ServiceCfg.Log.Info("Arret du service ...");
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            CarteTPLibrary.ServiceCfg.Log.Info("Service arrêté");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            //Worker worker = new Worker();
            //Thread wThread = new Thread(worker.DoWork);

            //wThread.Start();
            CarteTPLibrary.CarteManager.DoCards(e.FullPath);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
using System.IO;

namespace CarteTPExtranet
{
    public static class ServiceCfg
    {
        private static bool _isConfigured;
        private static string _connectionString;
        private static string _inputFolderPath;
        private static string _outputFolderPath;
        private static string _tempFolder;


        public static readonly ILog Log = LogManager.GetLogger("logging");
        /// <summary>.
        /// Proprietés de configuration
        /// </summary>
        public static bool IsConfigured { get { return _isConfigured; } }

        public static string ConnectionString { get { return _connectionString; } }
        public static string InputFolderPath { get { return _inputFolderPath; } }
        public static string OutputFolderPath { get { return _outputFolderPath; } }
        public static string TempFolder { get { return _tempFolder; } }
        

        /// <summary>
        /// vérifie la configuration du service 
        /// </summary>
        public static void CheckConfiguration()
        {
            _isConfigured = true;
            try
            {
                Log.Info("Vérification du paramètrage service ...");
                if (!ConfigurationManager.AppSettings.HasKeys())
                { throw  new Exception("aucune clé de configuration déclarée");}

                //construit la liste des params attendus
                List<string> cfgList = new List<string>();
                //on a systématiquement pour "dataindex" la donnée "tagscan"
                cfgList.AddRange(("csBatch,inputfolder,outputfolder,tempfolder").Split(','));

                //on verifie les params 
                foreach (var p in cfgList)
                {
                    var param = string.Empty;

                    if (ConfigurationManager.AppSettings.AllKeys.Contains(p))
                    {
                        param = ConfigurationManager.AppSettings[p];
                    }
                    else
                    {
                        _isConfigured = false;
                        param = $"Clé {p} attendue non déclarée";
                    }

                    switch (p)
                    {
                        case "csBatch":
                            if (IsConfigured)
                            {
                                _connectionString = param;
                            }
                            Log.Info($"Chaîne de connexion : {param}");
                            break;
                        case "inputfolder":
                            if (IsConfigured)
                            {
                                _inputFolderPath = param.Equals(string.Empty) ? Directory.GetCurrentDirectory() : param;
                            }
                            Log.Info($"Répertoire d'entrée : {param}");
                            break;
                        case "outputfolder":
                            if (IsConfigured)
                            {
                                _outputFolderPath = param.Equals(string.Empty) ? Directory.GetCurrentDirectory() : param;
                            }
                            Log.Info($"Répertoire de sortie : {param}");
                            break;
                        case "tempfolder":
                            if (IsConfigured)
                            {
                                //comme on respecte l'ordre de traitement de la liste cfgList
                                //OutputFolderPath sera l'alternative par défaut pour le répertoire temporaire
                                _tempFolder = param.Equals(string.Empty) ? Path.Combine(OutputFolderPath , "temp\\") : param;
                            }
                            Log.Info($"Répertoire temporaire : {param + (TempFolder.Equals(OutputFolderPath)? " (par défaut)" :string.Empty)}");
                            break;
                       
                        default:
                            throw new Exception($"la valeur clé '{p}' ne possède pas de cas de traitement");
                    }
                }
                Log.Info(IsConfigured ? "Paramétrage OK" : "Paramétrage KO");
            }
            catch (Exception e)
            {
                _isConfigured = false;
                Log.Error("ServiceCfg.CheckConfiguration : " + e.Message);
            }

        }

    }
}

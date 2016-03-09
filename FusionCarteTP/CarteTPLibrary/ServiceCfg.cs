using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
using System.IO;

namespace CarteTPLibrary
{
    public static class ServiceCfg
    {
        private static bool _isConfigured;
        private static string _connectionString;
        private static string _inputFolderPath;
        private static string _pdfModel;
        private static string _outputFolderPath;
        private static string _tempFolder;
        private static string _modelPageNumber;
        private static string _dataIndex;
        private static string _tagBegin;
        private static string _tagEnd;
        private static int _overlayX;
        private static int _overlayY;

        public static readonly ILog Log = LogManager.GetLogger("logging");
        /// <summary>.
        /// Proprietés de configuration
        /// </summary>
        public static bool IsConfigured { get { return _isConfigured; } }

        public static string ConnectionString { get { return _connectionString; } }
        public static string InputFolderPath { get { return _inputFolderPath; } }
        public static string PdfModel { get { return _pdfModel; } }
        public static string OutputFolderPath { get { return _outputFolderPath; } }
        public static string TempFolder { get { return _tempFolder; } }
        public static string ModelPageNumber { get { return _modelPageNumber; } }
        public static string DataIndex { get { return _dataIndex; } }
        public static string TagBegin { get { return _tagBegin; } }
        public static string TagEnd { get { return _tagEnd; } }
        public static int OverlayX { get { return _overlayX; } }
        public static int OverlayY { get { return _overlayY; } }

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
                cfgList.AddRange(("csBatch,inputfolder,outputfolder,tempfolder,model,modelpages,dataindex,overlayx,overlayy").Split(','));

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
                        case "model":
                            if (IsConfigured)
                            {
                                //ici le fichier model est obligatoire on lève une exception si ce dernier n'est pas déclaré
                                if(param.Equals(string.Empty))
                                    throw new Exception("fichier modèle n'est pas déclaré dans le fichier de configuration");
                                _pdfModel = param;
                            }
                            Log.Info($"Fichier modèle : {param}");
                            break;
                        case "modelpages":
                            if (IsConfigured)
                            {
                                //valeur 0 couvre toutes les pages du document modèle (par défaut)
                                _modelPageNumber = param.Equals(string.Empty) ? "0" : param;
                            }
                            Log.Info($"Page(s) du modèle à fusionner : {param + (ModelPageNumber.Equals("0") ? " (par défaut)" : string.Empty)}");
                            break;
                        case "dataindex":
                            if(IsConfigured)
                            {
                                if (ConfigurationManager.AppSettings.AllKeys.Contains("tagscan"))
                                {
                                    if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["tagscan"]))
                                    {
                                        throw new Exception("Valeur paramètre tagscan non définie");
                                    }
                                    else
                                    {
                                        _tagBegin = "<" + ConfigurationManager.AppSettings["tagscan"] + ">";
                                        _tagEnd = "</" + ConfigurationManager.AppSettings["tagscan"] + ">";
                                    }
                                }
                                else
                                {
                                    throw new Exception("Paramètre tagscan non défini");
                                }

                                //si une définition des positions à trouver "donnée:indice" séparées par ","
                                //sinon on scan le texte pour extraire l'info (par défaut)
                                _dataIndex = param.Equals(string.Empty) ? "scan" : param;
                                if(!param.Equals("scan"))
                                {
                                    //controle des definitions
                                    var lst = param.Split(',');
                                    if (lst.Length > 0)
                                    {
                                        foreach(var x in lst)
                                        {
                                            if (x.Split(':').Length < 2)
                                                throw new Exception("Paramètre DataIndex mal formaté : Format attendu [nom:valeur],[nom:valeur] ...");
                                        }
                                    }
                                    else
                                        throw new Exception("Paramètre DataIndex non défini");
                                }
                            }
                            Log.Info($"Positions des Données à extraire : {param}");
                            break;
                        case "overlayx":
                            if (IsConfigured)
                            {
                                //positionne à 0 par défaut
                                _overlayX = param.Equals(string.Empty) ? 0 : int.Parse(param);
                            }
                            Log.Info($"Position x du pdf texte : {param}");
                            break;
                        case "overlayy":
                            if (IsConfigured)
                            {
                                //positionne à 0 par défaut
                                _overlayY = param.Equals(string.Empty) ? 0 : int.Parse(param);
                            }
                            Log.Info($"Position y du pdf texte : {param}");
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

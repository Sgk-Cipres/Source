using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
using System.IO;

namespace CarteTPService
{
    public static class ServiceCfg
    {
        public static readonly ILog Log = LogManager.GetLogger("logging");
        /// <summary>.
        /// Proprietés de configuration
        /// </summary>
        public static bool IsConfigured;

        public static string ConnectionString;
        public static string InputFolderPath;
        public static string PdfModel;
        public static string OutputFolderPath;
        public static string TempFolder;
        public static string ModelPageNumber;
        public static int OverlayX;
        public static int OverlayY;

        /// <summary>
        /// vérifie la configuration du service 
        /// </summary>
        public static void CheckConfiguration()
        {
            IsConfigured = true;
            try
            {
                Log.Info("Vérification du paramètrage service ...");
                if (!ConfigurationManager.AppSettings.HasKeys())
                { throw  new Exception("aucune clé de configuration déclarée");}

                //construit la liste des params attendus
                List<string> cfgList = new List<string>();
                cfgList.AddRange(("csTherefore,inputfolder,outputfolder,tempfolder,model,modelpages,overlayx,overlayy").Split(','));

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
                        IsConfigured = false;
                        param = $"Clé {p} attendue non déclarée";
                    }

                    switch (p)
                    {
                        case "csTherefore":
                            if (IsConfigured)
                            {
                                ConnectionString = param;
                            }
                            Log.Info($"Chaîne de connexion : {param}");
                            break;
                        case "inputfolder":
                            if (IsConfigured)
                            {
                                InputFolderPath = param.Equals(string.Empty) ? Directory.GetCurrentDirectory() : param;
                            }
                            Log.Info($"Répertoire d'entrée : {param}");
                            break;
                        case "outputfolder":
                            if (IsConfigured)
                            {
                                OutputFolderPath = param.Equals(string.Empty) ? Directory.GetCurrentDirectory() : param;
                            }
                            Log.Info($"Répertoire de sortie : {param}");
                            break;
                        case "tempfolder":
                            if (IsConfigured)
                            {
                                //comme on respecte l'ordre de traitement de la liste cfgList
                                //OutputFolderPath sera l'alternative par défaut pour le répertoire temporaire
                                TempFolder = param.Equals(string.Empty) ? Path.Combine(OutputFolderPath , "temp\\") : param;
                            }
                            Log.Info($"Répertoire temporaire : {param + (TempFolder.Equals(OutputFolderPath)? " (par défaut)" :string.Empty)}");
                            break;
                        case "model":
                            if (IsConfigured)
                            {
                                //ici le fichier model est obligatoire on lève une exception si ce dernier n'est pas déclaré
                                if(param.Equals(string.Empty))
                                    throw new Exception("fichier modèle n'est pas déclaré dans le fichier de configuration");
                                PdfModel = param;
                            }
                            Log.Info($"Fichier modèle : {param}");
                            break;
                        case "modelpages":
                            if (IsConfigured)
                            {
                                //valeur 0 couvre toutes les pages du document modèle (par défaut)
                                ModelPageNumber = param.Equals(string.Empty) ? "0" : param;
                            }
                            Log.Info($"Page(s) du modèle à fusionner : {param + (ModelPageNumber.Equals("0") ? " (par défaut)" : string.Empty)}");
                            break;
                        case "overlayx":
                            if (IsConfigured)
                            {
                                //positionne à 0 par défaut
                                OverlayX = param.Equals(string.Empty) ? 0 : int.Parse(param);
                            }
                            Log.Info($"Position x du pdf texte : {param}");
                            break;
                        case "overlayy":
                            if (IsConfigured)
                            {
                                //positionne à 0 par défaut
                                OverlayY = param.Equals(string.Empty) ? 0 : int.Parse(param);
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
                IsConfigured = false;
                Log.Error("ServiceCfg.CheckConfiguration : " + e.Message);
            }

        }

    }
}

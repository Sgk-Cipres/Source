using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteTPLibrary
{
    public static class DataManager
    {
        private static readonly int _barCodLength = 24;
        private static readonly string[] _barCodTags = new string[4] { "<BARCOD>", "</BARCOD>", "<BARCOD2>", "</BARCOD2>" };

        private static string _nir13;
        private static string _cle;
        private static string _dateEdition;
        private static string _edition;
        private static string _refPli;
        private static bool _hasChanged;
        
        private static List<Carte> _cartes = new List<Carte>();

        public static string Nir13 { get { return _nir13; } }
        public static string Cle { get { return _cle; } }
        public static string DateEdition { get { return _dateEdition; } }
        public static string Edition { get { return _edition; } }
        public static string RefPli { get { return _refPli; } }
        public static string AssureP { get { return Dico.Keys.Contains(LogTableParam.Tiers)? Dico[LogTableParam.Tiers] : string.Empty; } }
        public static bool HasChanged { get { return _hasChanged; } }
        public static List<Carte> Cartes { get { return _cartes; } }
        public static Dictionary<string, string> Dico = new Dictionary<string, string>();
        public static string Enveloppe { get {
                return string.IsNullOrEmpty(AssureP)? string.Empty : 
                    !string.IsNullOrEmpty(DateEdition) && PdfManager.CheckFolder(Path.Combine(ServiceCfg.OutputFolderPath,
                    DateTime.Parse(DateEdition).ToString("yyyyMMdd")),true) ? Path.Combine(ServiceCfg.OutputFolderPath, 
                    DateTime.Parse(DateEdition).ToString("yyyyMMdd"), 
                    $"CTP_{AssureP}_{RefPli}_{DateTime.Parse(DateEdition).ToString("yyyyMMdd")}.pdf") : string.Empty;
            } }

        /// <summary>
        /// recherche des données exploitables dans un tableau de texte
        /// </summary>
        /// <param name="dataStrings"></param>
        public static void PrepareData(string[] dataStrings)
        {
            try
            {
                if (ServiceCfg.DataIndex.Equals("scan"))
                {
                    ExtractCarte(dataStrings);

                    var reference = dataStrings.First(x => x.Contains(ServiceCfg.TagBegin));
                    if (string.IsNullOrEmpty(reference))
                    {
                        throw new Exception($"Pas de balise xml {ServiceCfg.TagBegin}{ServiceCfg.TagEnd}");
                    }
                    //on enlève la balise début et fin dans la chaine
                    reference = reference.Replace(ServiceCfg.TagBegin, string.Empty).Replace(ServiceCfg.TagEnd, string.Empty);
                    _hasChanged = !reference.Equals(RefPli);
                    //matcher avec la référence si traitement sur le même pli
                    //on ne fait rien sinon on reactualise les infos
                    if (_hasChanged)
                    {
                        InitDico();
                        _refPli = reference;
                        _nir13 = string.Empty;
                        _cle = string.Empty;
                        _dateEdition = string.Empty;
                        _edition = string.Empty;
                        _cartes.Clear();

                        //on recherche les infos
                        foreach (var s in dataStrings)
                        {
                            if (string.IsNullOrEmpty(Nir13) || string.IsNullOrEmpty(DateEdition) || string.IsNullOrEmpty(Edition))
                            {
                                //on cherche un contenu ayant la date d'edition du document
                                if (s.Contains("Edité") && (string.IsNullOrEmpty(DateEdition) || string.IsNullOrEmpty(Edition)))
                                {
                                    _edition = s;
                                    _dateEdition = ExtractDate(s);
                                }
                                else
                                {
                                    //si on a pas de Nir13
                                    if (string.IsNullOrEmpty(Nir13))
                                    {
                                        //ici on va récupéré le numero INSEE de l'adhérent 
                                        //qui doit faire partie des informations contenues dans le document
                                        //(information qui doit etre contenue dans une ligne et isolée)
                                        var num = string.Empty;

                                        //on match les chaines de 13 caractères maximum (NIR sans clé)
                                        if (s.Replace(" ", "").Length == 13)
                                        {
                                            //on extrait de la chaine un potentiel NIR
                                            if (!string.IsNullOrEmpty(NumeroINSEE.MatchNumeroInsee(s)))
                                            {
                                                num = NumeroINSEE.NettoyerNumero(NumeroINSEE.MatchNumeroInsee(s));
                                                _cle = NumeroINSEE.CalculerCleINSEE(num).ToString("D2");
                                                if (NumeroINSEE.VerifierINSEE(num + Cle))
                                                {
                                                    //un NIR est trouvé on le garde
                                                    _nir13 = num;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        //on récupère l'info tiers id assuré principal
                    }
                }
                else
                {
                    //traitement par référence
                    var reference = dataStrings.First(x => x.Contains(ServiceCfg.TagBegin));
                    if (string.IsNullOrEmpty(reference))
                    {
                        throw new Exception($"Pas de balise xml {ServiceCfg.TagBegin}{ServiceCfg.TagEnd}");
                    }
                    //on enlève la balise début et fin dans la chaine
                    reference = reference.Replace(ServiceCfg.TagBegin, string.Empty).Replace(ServiceCfg.TagEnd, string.Empty);
                    _hasChanged = !reference.Equals(RefPli);

                    //var list = ServiceCfg.DataIndex.Split(',');
                    //var pli = list.First(x => x.Contains("pli"));
                    //if (string.IsNullOrEmpty(pli))
                    //    throw new Exception("Pas de paramètre 'pli'");

                    ////si on change de référence pli
                    //_hasChanged = !dataStrings[int.Parse(pli.Split(':')[1])].Equals(RefPli);
                    if (HasChanged)
                    {
                        _cartes.Clear();
                        _refPli = reference;
                    }

                    //foreach (var data in list)
                    //{
                    //    //donnée / position : name / value
                    //    string name = data.Split(':')[0];
                    //    int value = int.Parse(data.Split(':')[1]);

                    //    switch (name)
                    //    {
                    //        case "pli":
                    //            if (HasChanged)
                    //                _refPli = dataStrings[value];
                    //            break;
                    //        case "nni":
                    //            if (HasChanged)
                    //                _nir13 = dataStrings[value];
                    //                _cle = NumeroINSEE.CalculerCleINSEE(dataStrings[value]).ToString("D2");
                    //            break;
                    //        case "edit":
                    //            if (HasChanged)
                    //                _edition = dataStrings[value];
                    //                _dateEdition = ExtractDate(dataStrings[value]);
                    //            break;
                    //        case "barcod":
                    //        case "barcod2":
                    //            ExtractCarte(new string[] { dataStrings[value] });
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    string[] codes = dataStrings.Where(x => x.Contains(_barCodTags[0]) || x.Contains(_barCodTags[2])).ToArray();
                    if (codes.Length > 0)
                    {
                        ExtractCarte(codes);
                        //on récupère l'info tiers id assuré principal
                        if(HasChanged)
                            GetDataTiers();
                    }


                }
                //si à la fin de la recherche on a rien obtenu on log en erreur
                if (string.IsNullOrEmpty(Nir13))
                    throw new Exception("Aucune information exploitable dans le document");
            }
            catch (Exception ex)
            {
                ServiceCfg.Log.Error("DataManager.PrepareData : ", ex);
                throw new Exception("DataManager.PrepareData : ", ex);
            }
        }

        /// <summary>
        /// enregistre dans la table de log l'état et le message en paramètre
        /// </summary>
        /// <param name="etat">état du traitement</param>
        /// <param name="message">message retour traitement</param>
        public static void SetLogTable(int etat, string message)
        {
            SetLogTable(GetLastCardsSerial(), RefPli, message, etat, Dico);
        }

        /// <summary>
        /// actualise ou insère une ligne de log dans la table prévue à cet effet
        /// </summary>
        /// <param name="cartes">numero de serie des 2 cartes séparé par ','</param>
        /// <param name="pli">numero référence de pli</param>
        /// <param name="message">message à inserer dans la table de log</param>
        /// <param name="etat">état du traitement, -1: en erreur, 0: non traité, 1: traité</param>
        /// <param name="dico">dictionnaire avec des valeurs suplémentaires non obligatoires telles que 
        /// @tiers, @source, @page, @edition, @enveloppe, @intermediaire, @xml</param>
        public static void SetLogTable(string cartes, string pli, string message, int etat, Dictionary<string,string> dico)
        {
            //connexion à la base
            using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand("SetCarteTPLog", connection);

                command.CommandType = CommandType.StoredProcedure;

                //ajout des parametres de requête
                command.Parameters.Add(new SqlParameter(LogTableParam.Cartes, cartes));
                command.Parameters.Add(new SqlParameter(LogTableParam.Pli, pli));
                command.Parameters.Add(new SqlParameter(LogTableParam.Etat, etat));
                command.Parameters.Add(new SqlParameter(LogTableParam.Message, message));
                //si un dictionnaire existe on implémente les parametres définis
                if (dico != null && dico.Count > 0)
                {
                    foreach(var k in dico.Keys)
                    {
                        if(k.Equals(LogTableParam.Edition) && !string.IsNullOrEmpty(dico[k]))
                        {
                            var val = DateTime.Parse(dico[k]);
                            command.Parameters.Add(new SqlParameter(k, val));
                        }
                        else
                            command.Parameters.Add(new SqlParameter(k, dico[k]));
                    }
                }

                try
                {
                    //execution de la commande
                    if (command.ExecuteNonQuery() == 0)
                    {
                        connection.Close();
                        throw new Exception($"La mise à jour n'a pas impacté de lignes : Reference pli [{RefPli}] carte [{Cartes.ElementAt(0).Serial}]");
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    ServiceCfg.Log.Error("DataManager.SetLogTable : ", e);
                }
            }
        }

        /// <summary>
        /// obtien des infos sur le tiers assuré principal
        /// elles sont stockées par l'objet DataManager
        /// </summary>
        public static void GetDataTiers()
        {
            //connexion à la base
            using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand("GetTierDataByRef", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@pli", RefPli));

                var numCarte = GetLastCardsSerial().Split(',')[0];
                var carte = DataManager.Cartes.FirstOrDefault(x => x.Serial == numCarte);
                
                command.Parameters.Add(new SqlParameter("@serial", numCarte));

                try
                {
                    //execution de la commande
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //si un résultat existe
                        if (reader.HasRows)
                        {
                            reader.Read();
                            //on garde id tiers
                            SetDicoValue(LogTableParam.Tiers, reader["id_tiers"].ToString());
                            _nir13 = reader["nni"].ToString();
                            _cle = NumeroINSEE.CalculerCleINSEE(_nir13).ToString("D2");
                            _dateEdition = reader["date_edition"].ToString();
                            SetDicoValue(LogTableParam.Edition, DateEdition);
                            SetDicoValue(LogTableParam.Enveloppe, Enveloppe);
                        }
                        else
                        {
                            throw new Exception($"Aucune donnée trouvée en base concernant le pli référence [{DataManager.RefPli}] carte [{numCarte}] {Environment.NewLine}(voir fichier '{carte.SourceFile}' page {carte.SourcePage})");
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceCfg.Log.Error("DataManager.GetDataTiers : ", e);
                    throw new Exception($"DataManager.GetDataTiers : {e.GetBaseException().Message}", e);
                }
            }
        }

        /// <summary>
        /// obtien les infos pour la generation xml
        /// </summary>
        public static void GetData4Xml()
        {
            //connexion à la base
            using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
            {
                //ouverture
                connection.Open();

                //commande sql
                SqlCommand command = new SqlCommand("GetTierDataByInsee", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@insee", Nir13));

                try
                {
                    //execution de la commande
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //si un résultat existe
                        if (reader.HasRows)
                        {
                            XmlManager.CreateXml(reader);
                        }
                        else
                        {
                            throw new Exception($"Aucune donnée trouvée en base concernant '{DataManager.Nir13 + DataManager.Cle}' voir fichier '{PdfManager.LastPdf}'");
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceCfg.Log.Error("DataManager.GetData4Xml : ", e);
                    throw new Exception($"DataManager.GetData4Xml : {e.GetBaseException().Message}", e);
                }
            }
        }

        /// <summary>
        /// obtien le ou les numeros de serie des cartes traités (2 par page maximum)
        /// </summary>
        /// <returns>les numeros séparés par une ','</returns>
        public static string GetLastCardsSerial()
        {
            string serial = string.Empty;

            if(Cartes.Count > 0)
            {
                if(Cartes.Count == 1)
                {
                    serial = Cartes[0].Serial;
                }
                else
                {
                    serial = $"{Cartes[Cartes.Count - 2].Serial},{Cartes[Cartes.Count - 1].Serial}";
                }
            }

            return serial;
        }

        /// <summary>
        /// Ajoute ou modifie une valeur du dictionnaire DataManager
        /// le Dictionnaire détien les informations nécessaires à la table de log
        /// </summary>
        /// <param name="key">clé paramètre de procédure stockée (objet LogTableParam)</param>
        /// <param name="value">valeur</param>
        public static void SetDicoValue(string key,string value)
        {
            if(Dico.Keys.Contains(key))
            {
                Dico[key] = value;
            }
            else
            {
                Dico.Add(key, value);
            }
        }

        /// <summary>
        /// Initialise le dictionnaire avec des valeurs à vide
        /// la clé LogTableParam.Source est volontairement exclue 
        /// car elle est définie avant toutes les autres
        /// </summary>
        public static void InitDico()
        {
            SetDicoValue(LogTableParam.Tiers, string.Empty);
            SetDicoValue(LogTableParam.Page, string.Empty);
            SetDicoValue(LogTableParam.Edition, string.Empty);
            SetDicoValue(LogTableParam.Enveloppe, string.Empty);
            SetDicoValue(LogTableParam.Intermediaire, string.Empty);
            SetDicoValue(LogTableParam.Xml, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ExtractDate(string text)
        {
            string extracted = string.Empty;

            foreach (var part in text.Split(' '))
            {
                DateTime d = DateTime.Now;
                if (DateTime.TryParseExact(part, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out d))
                {
                    extracted =
                        DateTime.ParseExact(part, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                            .ToShortDateString();
                    break;
                }
            }

            return extracted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private static void ExtractCarte(string[] text)
        {
            foreach(var tag in _barCodTags)
            {
                var index = _barCodTags.ToList().IndexOf(tag);
                if(index%2 == 0)
                {
                    var barCodStr = text.FirstOrDefault(x => x.Contains(_barCodTags[index]));
                    if (!string.IsNullOrEmpty(barCodStr))
                    {
                        barCodStr = barCodStr.Replace(_barCodTags[index], string.Empty).Replace(_barCodTags[index+1], string.Empty);
                        if(barCodStr.Length >= _barCodLength)
                            _cartes.Add(new Carte(barCodStr) {
                                SourceFile = Dico.ContainsKey(LogTableParam.Source) ? Dico[LogTableParam.Source] : string.Empty,
                                SourcePage = Dico.ContainsKey(LogTableParam.Page) ? Dico[LogTableParam.Page] : string.Empty
                            });
                    }
                }
            }
        }
    }
}

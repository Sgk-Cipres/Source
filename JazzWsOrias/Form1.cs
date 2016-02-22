using System;
using System.Xml;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Orias.fr.orias.ws;

namespace Orias
{
    /// <summary>
    /// L'application attend 2 arguments dans la ligne de commande :
    /// Le premier défini la base de données à utilisé
    /// La seconde le type d'opération
    /// <remarks>
    /// Mise à jour du 24/11/2015
    /// Ajout de la base KLESIA dans les constantes ligne de commande et connectionString
    /// Ajout du compte d'accès au web service ORIAS dans AppConfig : DJO3J4VD2W59OF8NCNLD
    /// AF (IQ 2.0) rebuild code et correctifs 18/02/2016
    /// </remarks>
    /// <see cref="Constantes"/>
    /// </summary>
    public partial class ControleOrias : Form
    {
        private string logPath = @"C:\temp\";
        private string OriasAccount = Orias.Properties.Settings.Default.OriasUserAccount;
        
        private fr.orias.ws.CategoryName[] categoryOrias = new CategoryName[5]
        {
            fr.orias.ws.CategoryName.COA,
            fr.orias.ws.CategoryName.AGA,
            fr.orias.ws.CategoryName.MA,
            fr.orias.ws.CategoryName.MAL,
            fr.orias.ws.CategoryName.MIA
        };

        string[] argumentsLigneCmd = Environment.GetCommandLineArgs();
        private System.Data.OleDb.OleDbConnection SQLConnJazz = new System.Data.OleDb.OleDbConnection();
        private System.Data.OleDb.OleDbCommand SQLCommandInsertJazz = new System.Data.OleDb.OleDbCommand("INSERT INTO UsersExtention (ArgumentUser, ArgumentValue, UserId) VALUES (16,?,?)");
        private System.Data.OleDb.OleDbCommand SQLCommandUpdateJazz = new System.Data.OleDb.OleDbCommand("UPDATE UsersExtention SET ArgumentValue=? WHERE (UserId=?) AND (ArgumentUser = 16)");

        /// <summary>
        /// constructeur
        /// </summary>
        public ControleOrias()
        {
            InitializeComponent();
            if (argumentsLigneCmd.Count() > 2)
            {
                //On selectionne l'environnement - Par défaut on part sur l'environnement de recette
                switch (argumentsLigneCmd[1].ToString())
                {
                    case Constantes.CONST_ARGCMDLINE_DBRECETTE:
                        SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzRecette_ConnectionString;
                        comboBoxEnvironnement.SelectedIndex = 0;
                        break;
                    case Constantes.CONST_ARGCMDLINE_DBPREPROD:
                        SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzPrePro_ConnectionString;
                        comboBoxEnvironnement.SelectedIndex = 1;
                        break;
                    case Constantes.CONST_ARGCMDLINE_DBPROD:
                        SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzProd_ConnectionString;
                        comboBoxEnvironnement.SelectedIndex = 2;
                        break;
                    case Constantes.CONST_ARGCMDLINE_KLESIA:
                        SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzKlesia_ConnectionString;
                        comboBoxEnvironnement.SelectedIndex = 2;
                        break;
                    default:
                        SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzRecette_ConnectionString;
                        comboBoxEnvironnement.SelectedIndex = 0;
                        break;
                }
                switch (argumentsLigneCmd[2].ToString())
                {
                    case Constantes.CONST_ARGCMDLINE_AJOUT_NOUVEAU:
                        checkBoxMettreAJour.Checked = true;
                        break;
                    case Constantes.CONST_ARGCMDLINE_MAJ_COMPLETE:
                        checkBoxInserer.Checked = true;
                        checkBoxMettreAJour.Checked = true;
                        break;
                }
                comboBoxEnvironnement.Refresh();
            }
        }

        /// <summary>
        /// reinitialise le controle
        /// </summary>
        private void ReInitControl()
        {
            this.insertCategorieTableAdapter.Connection = SQLConnJazz;
            this.updateCategorieTableAdapter.Connection = SQLConnJazz;

            // SQLConnJazz.Open();

            SQLCommandInsertJazz.CommandType = CommandType.Text;
            SQLCommandUpdateJazz.CommandType = CommandType.Text;

            SQLCommandInsertJazz.Connection = SQLConnJazz;
            SQLCommandUpdateJazz.Connection = SQLConnJazz;


            // TODO: This line of code loads data into the 'jazzOrias.InsertCategorie' table. You can move, or remove it, as needed.
            this.insertCategorieTableAdapter.Fill(this.jazzOrias.InsertCategorie);
            // TODO: This line of code loads data into the 'jazzOrias.UpdateCategorie' table. You can move, or remove it, as needed.
            this.updateCategorieTableAdapter.Fill(this.jazzOrias.UpdateCategorie);

            AjoutAuLog(string.Concat(@"Connecté à la base : ", SQLConnJazz.Database.ToString()));
            this.Refresh();

        }

        /// <summary>
        /// chargement du controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControleOrias_Load(object sender, EventArgs e)
        {
            this.MajJazz();
            if (Orias.Properties.Settings.Default.ConsoleExecute)
                this.Close();
        }

        /// <summary>
        /// Gestion des environnements de connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxEnvironnement_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQLConnJazz.Close();
            switch (comboBoxEnvironnement.SelectedIndex)
            {
                case 0://CONST_ARGCMDLINE_DBRECETTE:
                    SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzRecette_ConnectionString;
                    break;
                case 1://CONST_ARGCMDLINE_DBPREPROD:
                    SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzPrePro_ConnectionString;
                    break;
                case 2://CONST_ARGCMDLINE_DBPROD:
                    SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzProd_ConnectionString;
                    break;
                case 3://CONST_ARGCMDLINE_KLESIA
                    SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzKlesia_ConnectionString;
                    break;
                default:
                    SQLConnJazz.ConnectionString = Orias.Properties.Settings.Default.JazzRecette_ConnectionString;
                    break;
            }

            try
            {
                SQLConnJazz.Open();
                this.ReInitControl();
                this.Refresh();
            }
            catch (Exception errConnection)
            {
                //this.BtnMiseAJourJazz.Enabled = false;
                this.AjoutAuLog(string.Concat("Erreur de connexion à la base (", SQLConnJazz.ConnectionString.ToString(), ") : ", errConnection.Message));
                this.Refresh();
            }
        }

        private void Click_BtnMiseAJourJazz(object sender, EventArgs e)
        {
            //MySoapCall();
            MajJazz();
        }

        /// <summary>
        /// ajout texte dans le log 
        /// </summary>
        /// <param name="log">texte à log</param>
        private void AjoutAuLog(string log)
        {
            this.txtLog.Text = string.Concat(this.txtLog.Text, System.DateTime.Now.ToString(), " - ", log, Environment.NewLine);
            this.txtLog.Refresh();
        }

        /// <summary>
        /// Retourne le libellé long d'une categorie Orias
        /// </summary>
        /// <example>COA = Courtier d'assurance ou de réassurance (COA)</example>
        /// <param name="cat">Category Orias</param>
        /// <returns>chaine de caractères </returns>
        private string CatergorieOriasLibelle(fr.orias.ws.CategoryName cat)
        {
            switch (cat)
            {
                case fr.orias.ws.CategoryName.COA:
                    return Constantes.CONST_ORIAS_INSCRIPTION_COA;
                //break;
                case fr.orias.ws.CategoryName.AGA:
                    return Constantes.CONST_ORIAS_INSCRIPTION_AGA;
                // break;
                case fr.orias.ws.CategoryName.MA:
                    return Constantes.CONST_ORIAS_INSCRIPTION_MA;
                //break;
                case fr.orias.ws.CategoryName.MAL:
                    return Constantes.CONST_ORIAS_INSCRIPTION_MAL;
                // break;
                case fr.orias.ws.CategoryName.MIA:
                    return Constantes.CONST_ORIAS_INSCRIPTION_MIA;
                //break;
                default:
                    return Constantes.CONST_ORIAS_INSCRIPTION_NONINSCRIT;
                //break;
            }

        }

        /// <summary>
        /// Mise à jour ORIAS
        /// </summary>
        private void MajJazz()
        {
            string nomFichierLog;

            txtLog.Text = string.Empty;

            AjoutAuLog(string.Concat(@"Connecté à la base : ", SQLConnJazz.Database.ToString()));

            #region Insertion des nouveaux courtiers

            if (checkBoxInserer.Checked)
            {
                AjoutAuLog("Début de la mise à jour des nouveaux courtiers.");
                InsertJazz();
            }

            #endregion

            #region Mise à jour des courtiers existants

            if (checkBoxMettreAJour.Checked)
            {
                AjoutAuLog("Début de la mise à jour de tous les courtiers.");
                UpdateJazz();
            }
            
            #endregion

            if (Directory.Exists(logPath))
            {
                nomFichierLog = Path.Combine(logPath, string.Concat(comboBoxEnvironnement.Text, "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"), ".rtf"));
            }
            else
            {
                Directory.CreateDirectory(logPath);
                nomFichierLog = Path.Combine(logPath, string.Concat(comboBoxEnvironnement.Text, "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"), ".rtf"));
            }

            txtLog.SaveFile(nomFichierLog);
        }

        /// <summary>
        /// Ajout Courtiers
        /// </summary>
        private void InsertJazz()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;

            fr.orias.ws.IntermediaryRequest[] oriasRequestCourtier = new fr.orias.ws.IntermediaryRequest[1];

            progressBar1.Maximum = jazzOrias.InsertCategorie.Count;

            foreach (JazzOrias.InsertCategorieRow rowOrias in jazzOrias.InsertCategorie)
            {
                Int32 idUser = (Int32)rowOrias["id"];

                progressBar1.Value += 1;
                txtBoxCodeOrias.Text = (progressBar1.Maximum - progressBar1.Value).ToString();
                this.txtBoxCodeOrias.Refresh();

                if (!string.IsNullOrEmpty(rowOrias["ArgumentValue"].ToString()))
                {
                    string libelleCategories = Constantes.CONST_ORIAS_INSCRIPTION_ENCOURS;
                    fr.orias.ws.IntermediaryRequest oriasInterRqtCourtier = new fr.orias.ws.IntermediaryRequest();
                    oriasInterRqtCourtier.Item = String.Format("{0,0:D8}", int.Parse(rowOrias["ArgumentValue"].ToString()));

                    //Ajout USER ORIAS
                    oriasRequestCourtier[0] = oriasInterRqtCourtier;

                    txtBoxCodeOrias.Text += @" " + oriasInterRqtCourtier.Item;
                    this.txtBoxCodeOrias.Refresh();

                    //24/11/2015 : Ajout USER
                    try
                    {
                        var searchIntermediaryRequest = new fr.orias.ws.intermediarySearchService();
                        fr.orias.ws.ArrayOfIntermediaryResponse searchReponse = (fr.orias.ws.ArrayOfIntermediaryResponse)searchIntermediaryRequest.intermediarySearch(OriasAccount, oriasRequestCourtier, categoryOrias);

                        foreach (fr.orias.ws.IntermediaryResponse courtier in searchReponse.intermediary)
                        {
                            if (courtier.informationBase.foundInRegistry)
                            {
                                foreach (var registration in courtier.registrations)
                                {
                                    if (registration.status == fr.orias.ws.RegistrationStatus.INSCRIT)
                                    {
                                        libelleCategories = libelleCategories == Constantes.CONST_ORIAS_INSCRIPTION_ENCOURS
                                            ? CatergorieOriasLibelle(registration.categoryName)
                                            : string.Concat(libelleCategories, @", ",
                                                CatergorieOriasLibelle(registration.categoryName));
                                    }
                                }
                            }
                        }

                        SQLCommandInsertJazz.Parameters.AddWithValue("LibelleCategories", libelleCategories);
                        SQLCommandInsertJazz.Parameters.AddWithValue("Courtier", idUser);
                        var r = SQLCommandInsertJazz.ExecuteNonQuery();
                        SQLCommandInsertJazz.Parameters.Clear();
                    }
                    catch (Exception oriasException)
                    {
                        AjoutAuLog(string.Concat(oriasInterRqtCourtier.Item, " ", oriasException.Message, Environment.NewLine));
                        break;
                    }
                }
            }
            AjoutAuLog(string.Concat("Fin de la mise à jour des nouveaux courtiers. Nombre de courtier mis à jour : ", progressBar1.Maximum.ToString()));
            
        }

        /// <summary>
        /// Mise à jour Courtiers
        /// </summary>
        private void UpdateJazz()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;
            fr.orias.ws.IntermediaryRequest[] oriasRequestCourtier = new fr.orias.ws.IntermediaryRequest[1];

            progressBar1.Maximum = jazzOrias.UpdateCategorie.Count;

            foreach (JazzOrias.UpdateCategorieRow rowOrias in jazzOrias.UpdateCategorie)
            {
                Int32 idUser = (Int32)rowOrias["id"];

                txtBoxCodeOrias.Refresh();
                progressBar1.Value += 1;
                txtBoxCodeOrias.Text = (progressBar1.Maximum - progressBar1.Value).ToString();

                if (!string.IsNullOrEmpty(rowOrias["ArgumentValue"].ToString()))
                {
                    string libelleCategories = Constantes.CONST_ORIAS_INSCRIPTION_ENCOURS;
                    fr.orias.ws.IntermediaryRequest oriasInterRqtCourtier = new fr.orias.ws.IntermediaryRequest();
                    oriasInterRqtCourtier.Item = String.Format("{0,0:D8}",
                        int.Parse(rowOrias["ArgumentValue"].ToString()));
                    oriasRequestCourtier[0] = oriasInterRqtCourtier;

                    txtBoxCodeOrias.Text += @" " + oriasInterRqtCourtier.Item;
                    this.txtBoxCodeOrias.Refresh();

                    try
                    {
                        var searchIntermediaryRequest = new fr.orias.ws.intermediarySearchService();
                        fr.orias.ws.ArrayOfIntermediaryResponse searchReponse =
                            (fr.orias.ws.ArrayOfIntermediaryResponse)
                                searchIntermediaryRequest.intermediarySearch(OriasAccount, oriasRequestCourtier,
                                    categoryOrias);

                        foreach (fr.orias.ws.IntermediaryResponse courtier in searchReponse.intermediary)
                        {
                            if (courtier.informationBase.foundInRegistry)
                            {
                                string courtierLog = string.Empty;
                                foreach (var registration in courtier.registrations)
                                {
                                    if (registration.status == fr.orias.ws.RegistrationStatus.INSCRIT)
                                    {
                                        if (libelleCategories == Constantes.CONST_ORIAS_INSCRIPTION_ENCOURS)
                                        {
                                            libelleCategories = CatergorieOriasLibelle(registration.categoryName);
                                            courtierLog = string.Concat(oriasInterRqtCourtier.Item, " ",
                                                CatergorieOriasLibelle(registration.categoryName),
                                                registration.registrationDate.ToString(" dd/MM/yyyy"));
                                        }
                                        else
                                        {
                                            libelleCategories = string.Concat(libelleCategories, ", ",
                                                CatergorieOriasLibelle(registration.categoryName));
                                            courtierLog = string.Concat(courtierLog, ", ",
                                                CatergorieOriasLibelle(registration.categoryName),
                                                registration.registrationDate.ToString(" dd/MM/yyyy"));
                                        }
                                    }
                                }
                                AjoutAuLog(courtierLog);
                            }
                        }

                        SQLCommandUpdateJazz.Parameters.AddWithValue("Categorie", libelleCategories);
                        SQLCommandUpdateJazz.Parameters.AddWithValue("Courtier", idUser);
                        var r = SQLCommandUpdateJazz.ExecuteNonQuery();
                        SQLCommandUpdateJazz.Parameters.Clear();

                    }
                    catch (Exception oriasException)
                    {
                        AjoutAuLog(string.Concat(oriasInterRqtCourtier.Item, " ", oriasException.Message, Environment.NewLine));
                        break;
                    }
                }
            }
            AjoutAuLog(string.Concat("Fin de la mise à jour de tous les courtiers. Nombre de courtier mis à jour : ", progressBar1.Maximum.ToString()));
        }

        /// <summary>
        /// sert à tester la communication au web service
        /// </summary>
        private void MySoapCall()
        {
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(
            @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:gpsa:orias:ws.001"">
               <soapenv:Header/>
               <soapenv:Body>
                  <urn:intermediarySearchRequest>
                     <user>DJO3J4VD2W59OF8NCNLD</user>
                     <intermediaries>
                        <intermediary>
                           <siren>552068199</siren>
                        </intermediary>
                     </intermediaries>
                      </urn:intermediarySearchRequest>
               </soapenv:Body>
            </soapenv:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    Console.WriteLine(soapResult);
                }
            }
        }

        /// <summary>
        /// genere entete d'une requête http
        /// </summary>
        /// <returns>objet requete http</returns>
        public HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://ws.orias.fr/service");
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        
    }
}
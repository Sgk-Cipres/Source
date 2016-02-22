using log4net;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportTelephonique
{
    public static class ReportingManager
    {
        private static readonly ILog Log = LogManager.GetLogger("logging");
        private static string basePath = ConfigurationManager.AppSettings["basepath"];
        private static string folder = ConfigurationManager.AppSettings["folder"];
        private static string connectionString = ConfigurationManager.AppSettings["csTherefore"];

        public static void GenerateReport(DataSet data)
        {
            try
            {
                using (ReportViewer reportViewer1 = new ReportViewer())
                {
                    reportViewer1.LocalReport.ReportEmbeddedResource = "ReportTelephonique.ReportModel.rdlc";

                    string filename = data.Tables["EventData"].Rows[0]["fichier"].ToString() + ".pdf";

                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string filenameExtension;

                    //liaison du report avec la source de données
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetReport", data.Tables["EventData"]));

                    reportViewer1.RefreshReport();
                    byte[] byteViewer = reportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                    var dir = basePath + folder;  // emplacement repertoire

                    if (!Directory.Exists(dir))  // si ça n'existe pas on le créé
                        Directory.CreateDirectory(dir);

                    //enregistrement du fichier
                    using (FileStream newFile = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        newFile.Write(byteViewer, 0, byteViewer.Length);
                        newFile.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("GenerateReport :", e);
            }
        }

        private static void LoadEventData(int id)
        {
            try
            { 
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //ouverture
                    connection.Open();

                    //commande sql
                    SqlCommand command = new SqlCommand("GetEventData", connection);

                    //délai d'attente requête 1 minute max
                    command.CommandTimeout = 60;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@id", id));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            DataSet ds = new DsReport();
                            ds.Tables["EventData"].Load(reader);

                            GenerateReport(ds);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("LoadEventData :", e);
            }

        }

        public static void ListEvents(string[] args)
        {
            try
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

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //ouverture
                    connection.Open();

                    //commande sql
                    SqlCommand command = new SqlCommand("GetEventIdList", connection);
                    //délai d'attente requête 1 minute max
                    command.CommandTimeout = 60;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@debut", debut));
                    command.Parameters.Add(new SqlParameter("@fin", fin));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                string filename = reader["id_pj"] as string;

                                var dir = basePath + folder;  // emplacement repertoire

                                if (!Directory.Exists(dir))  // si ça n'existe pas on le créé
                                    Directory.CreateDirectory(dir);

                                var files = Directory.EnumerateFiles(dir, "*" + filename + ".*");
                                //si un fichier correspond
                                if (!files.Any())
                                {
                                    LoadEventData(int.Parse(reader["id"].ToString()));
                                }
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("ListEvents :",e);
            }

        }

    }
}

using log4net.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteTPExtranet
{
    class Program
    {
        private static string _tiers = string.Empty;
        private static string _file = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();
                ServiceCfg.CheckConfiguration();

                //on récupère le pdf d'entrée

                //par requete base
                //connexion à la base
                using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
                {
                    //ouverture
                    connection.Open();

                    //commande sql
                    SqlCommand command = new SqlCommand("ListerRegroupTiersPli", connection);
                    //délai d'attente requête 3 minute max
                    command.CommandTimeout = 180;
                    command.CommandType = CommandType.StoredProcedure;


                    //execution de la commande
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //si un résultat existe
                        if (reader.HasRows)
                        {
                            //lecture de chaque ligne obtenue
                            while (reader.Read())
                            {
                                if(!_tiers.Equals(reader["IdTiersAssure"].ToString()))
                                {
                                    _tiers = reader["IdTiersAssure"].ToString();
                                    _file = Path.Combine(ServiceCfg.OutputFolderPath, $"EXT_{_tiers}.pdf");
                                }

                                if (reader["FichierEnveloppe"] != DBNull.Value)
                                {
                                    PdfManager.ConcatPdf(_file, reader["FichierEnveloppe"].ToString(), _file);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                ServiceCfg.Log.Error("Execution : ", e);
            }

        }
    }
}

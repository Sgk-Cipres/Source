
using CarteTPLibrary;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionCarteTP
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {

                CarteManager.Initialize();

                //on récupère le pdf d'entrée

                //par requete base
                //connexion à la base
                //using (SqlConnection connection = new SqlConnection(ServiceCfg.ConnectionString))
                //{
                //    //ouverture
                //    connection.Open();

                //    //commande sql
                //    SqlCommand command = new SqlCommand("ListerPdfLot", connection);
                //    //délai d'attente requête 3 minute max
                //    command.CommandTimeout = 180;
                //    command.CommandType = CommandType.StoredProcedure;


                //    //execution de la commande
                //    using (SqlDataReader reader = command.ExecuteReader())
                //    {
                //        //si un résultat existe
                //        if (reader.HasRows)
                //        {
                //            //lecture de chaque ligne obtenue
                //            while (reader.Read())
                //            {
                //                if (reader["FichierPDF"] != DBNull.Value)
                //                {
                //                    CarteManager.DoCards(reader["FichierPDF"].ToString());
                //                }
                //            }
                //        }
                //    }
                //}


                //par scrutation de répertoire
                if (PdfManager.CheckFolder(ServiceCfg.InputFolderPath, false))
                {
                    var lot = PdfManager.FindPdfFiles(ServiceCfg.InputFolderPath);

                    if (lot.Any())
                    {
                        foreach (var l in lot)
                        {
                            CarteManager.DoCards(l);
                            Console.Write("count{0}", count++);
                        }

                        //Parallel.ForEach(lot, (l) =>
                        //{
                        //    CarteManager.DoCards(l);
                        //    Console.Clear();
                        //    Console.Write("count{0}", count++);
                        //});

                    }
                }


            }
            catch(Exception e)
            {
                ServiceCfg.Log.Error("Execution : ", e);
            }

            Console.Read();

            watch.Stop();
            Console.WriteLine($"temps d'execution : {watch.ElapsedMilliseconds}");
        }
    }
}

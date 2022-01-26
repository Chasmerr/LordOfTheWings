using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using LordOfTheWings.DAL.Models;

namespace LordOfTheWings.DAL.Context
{
    public class Context
    {
        public const string projectId = "arc3-339316";
        BigQueryClient client;
        public static Context context = new Context();
        
        public Context()
        {
            var path = Environment.CurrentDirectory + @"\" + "service-account-file.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            client = BigQueryClient.Create(projectId);
        }

        public List<Reservation> GetAllReservations()
        {
            string query = @"select date,time,who,TableNumber from arc3-339316.reservations.reservation left join arc3-339316.tables.mycollections on reservation.table_id = mycollections.id";
            var result = client.ExecuteQuery(query, parameters: null);
            
            List<Reservation> res = new List<Reservation>();

            foreach(var row in result)
            {
                int tnumber;
                bool success = int.TryParse(row["TableNumber"].ToString(), out tnumber);

                res.Add(new Reservation()
                {
                    TableNumber = tnumber,
                    ReservedBy = row["who"].ToString(),
                    ReservationDate = row["date"].ToString().Substring(0, row["date"].ToString().IndexOf(' ')),
                    ReservationTime = row["time"].ToString()
                });
            }

            return res;
        }

        
    }
}

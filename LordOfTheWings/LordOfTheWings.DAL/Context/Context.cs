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
using Google.Cloud.Language.V1;
using LordOfTheWings.DAL.Models;

namespace LordOfTheWings.DAL.Context
{
    public class Context
    {
        const string projectId = "arc3-339316";
        BigQueryClient bqClient;
        LanguageServiceClient languageClient;
        public static Context context = new Context();
        
        public Context()
        {
            var path = Environment.CurrentDirectory + @"\" + "service-account-file.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            bqClient = BigQueryClient.Create(projectId);
            languageClient = LanguageServiceClient.Create();
        }

        public List<Reservation> GetAllReservations()
        {
            string query = @"select date,time,who,TableNumber from arc3-339316.reservations.reservation";
            var result = bqClient.ExecuteQuery(query, parameters: null);
            
            List<Reservation> res = new List<Reservation>();

            foreach(var row in result)
            {
                int tnumber;
                bool success = int.TryParse(row["TableNumber"].ToString(), out tnumber);
                if(success)
                {
                    res.Add(new Reservation()
                    {
                        TableNumber = tnumber,
                        ReservedBy = row["who"].ToString(),
                        ReservationDate = row["date"].ToString().Substring(0, row["date"].ToString().IndexOf(' ')),
                        ReservationTime = row["time"].ToString()
                    });
                }

                
            }

            return res;
        }

        public float GetOpinionPositivityRating(string content)
        {
            Document doc = Document.FromPlainText(content);
            AnalyzeSentimentResponse response = languageClient.AnalyzeSentiment(doc);
            var score = response.DocumentSentiment.Score;
            return score; 
        }

        public void SendOpinion(Opinion opinion)
        {
            string query = @"insert into arc3-339316.opinions.opinion(content,date,positivity) values (@content, @date, @positivity)";

            var parameters = new BigQueryParameter[]
            {
                new BigQueryParameter("content", BigQueryDbType.String, opinion.content),
                new BigQueryParameter("date", BigQueryDbType.Date, opinion.date),
                new BigQueryParameter("positivity", BigQueryDbType.Float64, opinion.positivity)
            };

            bqClient.ExecuteQuery(query, parameters);
        }

        public void MakeReservation(Reservation reservation)
        {
            string query = "insert into arc3-339316.reservations.reservation(date, time, who, TableNumber) values('" + reservation.ReservationDate + "','" + reservation.ReservationTime + ":00','" + reservation.ReservedBy + "'," + reservation.TableNumber + ")";
            bqClient.ExecuteQuery(query, parameters: null);
        }

        //public List<Order> getAllOrders()
        //{
        //    string query = @"";
        //    var result = bqClient.ExecuteQuery(query, parameters: null);

        //    List<Order> res = new List<Order>();

        //    foreach(var row in result)
        //    {
        //        int tnumber;
        //        bool success = int.TryParse(row["TableNumber"].ToString(), out tnumber);


        //        if (success)
        //        {
        //            res.Add(new Order()
        //            {
        //                tableNumber = tnumber
        //            });
        //        }
        //    }
        //    return null;
        //}
    }
}

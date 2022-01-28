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

        public List<Opinion> GetAllOpinions()
        {
            string query = @"select * from arc3-339316.opinions.opinion";
            var result = bqClient.ExecuteQuery(query, parameters: null);

            List<Opinion> res = new List<Opinion>();

            foreach (var row in result)
            {
                float pos;
                bool success = float.TryParse(row["positivity"].ToString(), out pos);
                if (success)
                {
                    res.Add(new Opinion()
                    {
                        content = row["content"].ToString(),
                        date = row["date"].ToString(),
                        positivity = pos
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
            string query = "insert into arc3-339316.reservations.reservation(date, time, who, TableNumber) values(@ReservationDate, @ReservationTime, @ReservedBy, @TableNumber)";
            var parameters = new BigQueryParameter[]
            {
                new BigQueryParameter("ReservationDate", BigQueryDbType.Date, reservation.ReservationDate),
                new BigQueryParameter("ReservationTime", BigQueryDbType.Time, reservation.ReservationTime + ":00"),
                new BigQueryParameter("ReservedBy", BigQueryDbType.String, reservation.ReservedBy),
                new BigQueryParameter("TableNumber", BigQueryDbType.Int64, reservation.TableNumber)
            };
            bqClient.ExecuteQuery(query, parameters);
        }

        public List<DishPopularityChartItem> GetOrderedDishesWithCount()
        {
            string query = @"select name, count(name) as count from `arc3-339316.orders.orderedDishes` as orderedDishes join `arc3-339316.dishes.mycollections` as dishes on orderedDishes.DishId = dishes.id group by name";
            var result = bqClient.ExecuteQuery(query, parameters: null);

            List<DishPopularityChartItem> res = new List<DishPopularityChartItem>();

            foreach(var row in result)
            {
                int count;
                bool success = int.TryParse(row["count"].ToString(), out count);

                if(success)
                {
                    res.Add(new DishPopularityChartItem()
                    {
                        DishName = row["name"].ToString(),
                        Count = count
                    });
                }
            }

            return res;
        }
        public List<OpinionChartItem> GetOpinionsSentimentCount()
        {
            string query = @"select positivity from arc3-339316.opinions.opinion";
            var result = bqClient.ExecuteQuery(query, parameters: null);

            List<OpinionChartItem> res = new List<OpinionChartItem>()
            {
                new OpinionChartItem()
                {
                    Positivity = "Positive",
                    Count = 0,
                },
                new OpinionChartItem()
                {
                    Positivity = "Negative",
                    Count = 0,
                },
                new OpinionChartItem()
                {
                    Positivity = "Neutral",
                    Count = 0,
                },
            };

            List<float> f = new List<float>();

            foreach (var row in result)
            {
                float pos;
                bool success = float.TryParse(row["positivity"].ToString(), out pos);

                if (success)
                {
                    if (pos > 0.2)
                        res.First(x => x.Positivity == "Positive").Count++;
                    else if (pos <= 0.2 && pos >= -0.2)
                        res.First(x => x.Positivity == "Neutral").Count++;
                    else
                        res.First(x => x.Positivity == "Negative").Count++;
                }
            }

            return res;
        }

        public List<TablePopularityChartItem> GetReservedTablesWithCount()
        {
            string query = @"select TableNumber, count(TableNumber) as count from arc3-339316.reservations.reservation group by TableNumber";
            var result = bqClient.ExecuteQuery(query, parameters: null);

            List<TablePopularityChartItem> res = new List<TablePopularityChartItem>();

            foreach (var row in result)
            {
                int count;
                int tNumber;
                bool success1 = int.TryParse(row["count"].ToString(), out count);
                bool success2 = int.TryParse(row["TableNumber"].ToString(), out tNumber);

                if (success1 && success2)
                {
                    res.Add(new TablePopularityChartItem()
                    {
                        TableNumber = tNumber,
                        Count = count
                    });
                }
            }

            return res;
        }

        public List<HourPopularityChartItem> GetOrderHoursWithCount()
        {
            string query = @"select orderHour, count(orderHour) as count from (select extract(HOUR from OrderTime) as orderHour from orders.order) group by orderHour order by orderHour";
            var result = bqClient.ExecuteQuery(query, parameters: null);

            List<HourPopularityChartItem> res = new List<HourPopularityChartItem>();

            foreach (var row in result)
            {
                int hour;
                int count;
                bool success2 = int.TryParse(row["orderHour"].ToString(), out hour);
                bool success1 = int.TryParse(row["count"].ToString(), out count);

                if (success1 && success2)
                {
                    res.Add(new HourPopularityChartItem()
                    {
                        Hour = hour,
                        Count = count
                    });
                }
            }

            return res;
        }
    }
}

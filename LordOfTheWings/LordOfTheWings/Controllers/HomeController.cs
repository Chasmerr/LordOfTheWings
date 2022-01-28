using LordOfTheWings.DAL.Context;
using LordOfTheWings.DAL.Models;
using LordOfTheWings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.DataTable.Net.Wrapper;

namespace LordOfTheWings.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string GetDishPopularityPieChartJSON()
        {
            List<DishPopularityChartItem> items = Context.context.GetOrderedDishesWithCount();

            var dt = new DataTable();
            dt.AddColumn(new Column(ColumnType.String, "DishName", "Dish name"));
            dt.AddColumn(new Column(ColumnType.Number, "Count", "Count"));

            foreach(var item in items)
            {
                Row r = dt.NewRow();
                r.AddCellRange(new Cell[]
                {
                    new Cell(item.DishName),
                    new Cell(item.Count)
                });
                dt.AddRow(r);
            }

            return dt.GetJson();
        }
        public string GetTablePopularityPieChartJSON()
        {
            List<TablePopularityChartItem> items = Context.context.GetReservedTablesWithCount();

            var dt = new DataTable();
            dt.AddColumn(new Column(ColumnType.String, "TableNumber", "Table number"));
            dt.AddColumn(new Column(ColumnType.Number, "Count", "Count"));

            foreach (var item in items)
            {
                Row r = dt.NewRow();
                r.AddCellRange(new Cell[]
                {
                    new Cell(item.TableNumber.ToString()),
                    new Cell(item.Count)
                });
                dt.AddRow(r);
            }

            return dt.GetJson();
        }
        public string GetOpinionChartJSON()
        {
            List<OpinionChartItem> items = Context.context.GetOpinionsSentimentCount();

            var dt = new DataTable();
            dt.AddColumn(new Column(ColumnType.String, "Positivity", "Positivity"));
            dt.AddColumn(new Column(ColumnType.Number, "Count", "Count"));

            foreach (var item in items)
            {
                Row r = dt.NewRow();
                r.AddCellRange(new Cell[]
                {
                    new Cell(item.Positivity),
                    new Cell(item.Count)
                });
                dt.AddRow(r);
            }

            return dt.GetJson();
        }

        public string GetHourPopularityChartJSON()
        {
            List<HourPopularityChartItem> items = Context.context.GetOrderHoursWithCount();

            var dt = new DataTable();
            dt.AddColumn(new Column(ColumnType.Number, "Hour", "Godzina"));
            dt.AddColumn(new Column(ColumnType.Number, "Count", "Ilość zamówień"));

            foreach (var item in items)
            {
                Row r = dt.NewRow();
                r.AddCellRange(new Cell[]
                {
                    new Cell(item.Hour),
                    new Cell(item.Count)
                });
                dt.AddRow(r);
            }

            return dt.GetJson();
        }
    }
}

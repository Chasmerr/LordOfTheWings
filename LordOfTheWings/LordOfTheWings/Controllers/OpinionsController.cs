using LordOfTheWings.DAL.Context;
using LordOfTheWings.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LordOfTheWings.Controllers
{
    public class OpinionsController : Controller
    {
        public IActionResult Index()
        {
            var vm = Context.context.GetAllOpinions();
            return View(vm);
        }

        [HttpGet]
        public IActionResult CreateOpinion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOpinion(Opinion o)
        {
            DateTime dt = DateTime.Now;
            o.date = dt.ToString("yyyy-MM-dd");
            o.positivity = Context.context.GetOpinionPositivityRating(o.content);

            Context.context.SendOpinion(o);

            return RedirectToAction("OpinionSent");
        }

        public IActionResult OpinionSent()
        {
            return View();
        }
    }
}

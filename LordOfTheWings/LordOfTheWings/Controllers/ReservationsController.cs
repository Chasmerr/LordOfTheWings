using LordOfTheWings.DAL.Context;
using LordOfTheWings.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LordOfTheWings.Controllers
{
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            var vm = Context.context.GetAllReservations();
            return View(vm);
        }

        [HttpGet]
        public IActionResult MakeReservation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MakeReservation(Reservation reservation)
        {
            Context.context.MakeReservation(reservation);

            return RedirectToAction("ReservationMade");
        }

        public IActionResult ReservationMade()
        {
            return View();
        }
    }
}

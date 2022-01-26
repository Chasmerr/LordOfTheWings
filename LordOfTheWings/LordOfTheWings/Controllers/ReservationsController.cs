using LordOfTheWings.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LordOfTheWings.Controllers
{
    public class ReservationsController : Controller
    {
        

        public ReservationsController()
        {
            
        }

        public IActionResult Index()
        {
            var vm = Context.context.GetAllReservations();
            return View(vm);
        }
    }
}

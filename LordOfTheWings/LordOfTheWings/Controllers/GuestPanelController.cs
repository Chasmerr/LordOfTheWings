using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LordOfTheWings.Controllers
{
    public class GuestPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svr.AD.Extensions;
using Svr.AD.Models;
using Svr.Infrastructure.Extensions;

namespace Svr.AD.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Страница описания сервиса.";
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Наши контакты.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

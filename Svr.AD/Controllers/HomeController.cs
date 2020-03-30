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
        [AuthorizeRoles(Role.Admin, Role.User)]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(Role.User)]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }
        [AuthorizeRoles(Role.Admin)]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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

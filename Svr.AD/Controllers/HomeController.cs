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
        #region Index
        public IActionResult Index()
        {
            ViewData["Message"] = "";
            return View();
        }
        #endregion
        #region About
        public IActionResult About()
        {
            ViewData["Message"] = "Страница описания сервиса.";
            return View();
        }
        #endregion
        #region Contact
        public IActionResult Contact()
        {
            ViewData["Message"] = "Наши контакты.";

            return View();
        }
        #endregion
        #region Privacy
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion
        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}

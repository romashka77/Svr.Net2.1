using Microsoft.AspNetCore.Mvc;
using Svr.Web.Models;
using System.Diagnostics;

namespace Svr.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "";
            return View();
        }

        public IActionResult Test()
        {
            ViewData["Message"] = "Test.";

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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

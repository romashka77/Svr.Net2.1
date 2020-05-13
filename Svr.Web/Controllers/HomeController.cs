using Microsoft.AspNetCore.Mvc;
using Svr.Web.Models;
using System.Diagnostics;

namespace Svr.Web.Controllers
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
        #region Test
        public IActionResult Test()
        {
            ViewData["Message"] = "Test.";

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
        #region Error
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}

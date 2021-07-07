using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Svr.Utils.Controllers
{
    public class MessageController : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }
  //      public void SetReferer()
		//{
  //          if (!String.IsNullOrEmpty(Request.Headers["Referer"]))
  //          {
  //              ViewData["Reffer"] = Request.Headers["Referer"].ToString();
  //          }
  //      }
  //      protected void Details()
		//{
  //          SetReferer();
  //      }
    }
}

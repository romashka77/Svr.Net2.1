using Microsoft.AspNetCore.Mvc;

namespace Svr.Utils.Controllers
{
    public class MessageController : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}

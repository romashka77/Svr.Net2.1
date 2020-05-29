using Microsoft.AspNetCore.Mvc;

namespace Svr.Utils
{
    public class MessageController : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}

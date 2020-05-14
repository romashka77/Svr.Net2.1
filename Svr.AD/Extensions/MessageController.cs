using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.AD.Extensions
{
    public class MessageController : Controller
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}

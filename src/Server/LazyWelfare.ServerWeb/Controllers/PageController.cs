using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerWeb.Controllers
{
    public class PageController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CommandPanel()
        {
            return View();
        }
    }
}

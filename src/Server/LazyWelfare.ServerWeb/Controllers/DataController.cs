using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerWeb.Controllers
{
    public class DataController : Controller
    {

        public IActionResult ServiceDefine()
        {

            return View();
        }

    }
}

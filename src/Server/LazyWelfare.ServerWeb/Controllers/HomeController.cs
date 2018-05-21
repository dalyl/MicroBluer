using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LazyWelfare.ServerWeb.Models;

namespace LazyWelfare.ServerWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult QrCode()
        {
            var width = 100;
            var height = 100;
            var img= ServerCore.ImageCreate.QRCode(ServerCore.ServerEnvironment.Instance.WebAddress, width, height);
            byte[] byData = ServerCore.ImageCreate.Convert(img);
            return File(byData, "image/jpg");
        }
    }
}

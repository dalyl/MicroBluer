using LazyWelfare.ServerCore;
using LazyWelfare.ServerCore.CommandInterface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerWeb.Controllers
{
    public class VolumeController : Controller
    {

        public IVolumeController VolumeService { get; private set; }

        public VolumeController(IVolumeController service)
        {
            VolumeService = service;
        }

        public IActionResult Set(decimal value)
        {
            VolumeService.SetValue(value);
            return Json(new TaskResult(true));
        }

        public IActionResult Get()
        {
            var value = VolumeService.GetValue();
            return Json(new TaskResult<decimal>(value));
        }
    }

    

  
}

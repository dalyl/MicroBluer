using LazyWelfare.ServerCore.Command;
using LazyWelfare.ServerWeb.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerWeb.Controllers
{
    public class VolumeController : Controller
    {

        public IVolumeControl VolumeService { get; private set; }

        public VolumeController(IVolumeControl service)
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

using LazyWelfare.Interface;
using LazyWelfare.ServerCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerWeb.Controllers
{
    [Area("Command")]
    public class VolumeController : Controller
    {
        public static JsonSerializerSettings setting = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
        };

        public IVolumeController VolumeService { get; private set; }

        public VolumeController(IVolumeController service)
        {
            VolumeService = service;
        }

        public IActionResult Set(decimal value)
        {
            VolumeService.SetValue(value);
            return Json(new TaskResult(true), setting);
        }

        public IActionResult Get()
        {
            var value = VolumeService.GetValue();
            return Json(new TaskResult<decimal>(value), setting);
        }
    }

    

  
}

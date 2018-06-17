

namespace MicroBluer.ServerWeb.Controllers
{
    using MicroBluer.Interface;
    using MicroBluer.ServerCore;
    using Microsoft.AspNetCore.Mvc;

    [Area("Command")]
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

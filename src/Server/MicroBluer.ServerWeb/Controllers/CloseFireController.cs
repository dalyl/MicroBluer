namespace MicroBluer.ServerWeb.Controllers
{
    using MicroBluer.ServerCore;
    using MicroBluer.TimeTask;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class CloseFireController : Controller
    {
        public IActionResult set(string value)
        {
            var time = JsonConvert.DeserializeObject<TimeSetting>(value);
            return Json(new TaskResult(true));
        }
    }
}
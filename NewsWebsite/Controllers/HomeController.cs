using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsWebsite.Controllers
{
    public class HomeController : BaseViewsController
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            IExceptionHandlerPathFeature exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "An exception with the following input occured: HttpContext.Request.Form: {@Form}, HttpContext.Request.Query: {@Query} at the following path: {@Path}", JsonConvert.SerializeObject(HttpContext.Request.Form), JsonConvert.SerializeObject(HttpContext.Request.Query), exceptionHandlerPathFeature.Path);
            TempData["ErrorMessage"] = "Ooops, something went wrong";
            return RedirectToIndexActionInHomeController();
        }
    }
}

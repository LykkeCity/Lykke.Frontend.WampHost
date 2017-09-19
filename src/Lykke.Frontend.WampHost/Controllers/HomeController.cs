using Lykke.Frontend.WampHost.Models;
using Lykke.Frontend.WampHost.Services.Documentation;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Frontend.WampHost.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            var doc = new TypeDocGenerator();
            var model = new MethodInfoModel
            {
                Rpc = doc.GetDocumentation(typeof(IRpcFrontend)),
                Topic = doc.GetDocumentation(typeof(IWampTopics))
            };

            return View(model);
        }
    }
}
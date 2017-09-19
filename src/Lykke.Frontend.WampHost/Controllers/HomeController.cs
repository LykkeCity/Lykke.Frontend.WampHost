using Lykke.Frontend.WampHost.Documentation;
using Lykke.Frontend.WampHost.Models;
using Microsoft.AspNetCore.Mvc;
using WampSharp.V2.PubSub;

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
                Topic = doc.GetDocumentation(typeof(IWampTopics))
            };

            return View(model);
        }
    }
}
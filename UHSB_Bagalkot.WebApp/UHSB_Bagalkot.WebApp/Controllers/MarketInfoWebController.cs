using Microsoft.AspNetCore.Mvc;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class MarketInfoWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MarketIndex()
        {

            return View();
        }

    }
}

using Microsoft.AspNetCore.Mvc;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class fertilizerWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> fertilizerIndex()
        {

            return View();
        }

    }
}

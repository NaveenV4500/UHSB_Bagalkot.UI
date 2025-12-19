using Microsoft.AspNetCore.Mvc;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class QuestionnaireWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> QuestionnaireIndex()
        {

            return View();
        }

    }
}

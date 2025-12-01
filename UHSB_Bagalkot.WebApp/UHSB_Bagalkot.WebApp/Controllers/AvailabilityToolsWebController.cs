using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels.AvailabilityTools;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class AvailabilityToolsWebController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public AvailabilityToolsWebController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiSettings = apiSettings.Value;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CentersAvailabilityDetails()
        {
            var model = new AvailabilityToolsDetailsVM
            {
                Centerstype = new List<System.Web.Mvc.SelectListItem>(),
                Centervariatestype = new List<System.Web.Mvc.SelectListItem>()
            }; 
            ViewBag.fileBaseUrl = _apiSettings.File_Url;
            return View(model);
        }

        [HttpPost]
        public IActionResult CentersAvailabilityDetails(CentersAvailabilityDetailsVM model)
        {
            if (model == null)
            {
            }   

            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class WeatherReportFilesController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public WeatherReportFilesController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DistrictDD> districtList = new List<DistrictDD>();

            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/WeatherCast/DistrictDD");

            if (response.IsSuccessStatusCode)
            {
                districtList = await response.Content.ReadFromJsonAsync<List<DistrictDD>>();
            }

            ViewBag.Districts = districtList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(WeatherFileUploadVM model, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file selected" });


            if (file.Length > 0)
            {
                // Folder path: wwwroot/InwardsInvoices/TempFiles (you can change it)
                //var uploadPath = Path.Combine(env.WebRootPath, "InwardsInvoices", "TempFiles");

                var uploadPath = _apiSettings.UploadSettings.WeatherReportFilesPath;
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.FileName = Path.Combine("WeatherReportFiles", "TempFiles", fileName).Replace("\\", "/");
                model.UserId = 1;// Convert.ToInt16(HttpContext.Session.GetString("UserID"));
            }
            
            var json = System.Text.Json.JsonSerializer.Serialize(model);

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var apiUrl = $"{_apiSettings.Base_Url}/WeatherCast/upload";

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Json(new { success = true, data = result });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = error });
            }

        }
    }
}

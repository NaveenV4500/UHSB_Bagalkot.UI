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
            _httpClient = httpClientFactory.CreateClient("ApiClient");
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
            {
                TempData["Message"] = "No file selected.";
                return RedirectToAction("Index");
            }

            if (file.Length > 0)
            {
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
                model.UserId = 1;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var apiUrl = $"{_apiSettings.Base_Url}/WeatherCast/upload";
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "File uploaded successfully!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Message"] = $"Upload failed: {error}";
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> WeatherFilesDetails()
        {
            List<DistrictDD> districtList = new List<DistrictDD>();

            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/WeatherCast/DistrictDD");

            if (response.IsSuccessStatusCode)
            {
                districtList = await response.Content.ReadFromJsonAsync<List<DistrictDD>>();
            }
            ViewBag.fileBaseUrl = _apiSettings.File_Url;

            ViewBag.Districts = districtList;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> WeatherDistrictWiseDetails(int districtid = 0)
        {
            List<WeeklyWeatherReportGridVM> data = new List<WeeklyWeatherReportGridVM>();
            var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/WeatherCast/Weekly?districtId=" + districtid);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadFromJsonAsync<List<WeeklyWeatherReportGridVM>>();
            }
            ViewBag.fileBaseUrl = _apiSettings.File_Url;

            return View(data);
        }

    }
}

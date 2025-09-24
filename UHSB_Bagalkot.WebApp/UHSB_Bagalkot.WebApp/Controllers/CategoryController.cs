using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UHSB_Bagalkot.WebService.AppSettings;
using UHSB_Bagalkot.WebService.ViewModels.CategoryModels;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _httpClient.GetAsync("Category/getallCategory");
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(CategoryVM category)
        {
            var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Category/categoryAddOrEdit", content);
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"Category/{id}");
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}

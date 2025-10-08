using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class UserMasterController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public UserMasterController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiSettings = apiSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetGridContentV1(int currentPage = 1, int pageSize = 10, GridEnum.UserMasterColumns orderBy = GridEnum.UserMasterColumns.UserName, bool isDescending = false, string filterDetails = null, string externalFilter = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Unauthorized. Please login." });
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var query = new Dictionary<string, string>
                {
                    ["currentPage"] = currentPage.ToString(),
                    ["pageSize"] = pageSize.ToString(),
                    ["orderBy"] = orderBy.ToString(),
                    ["isDescending"] = isDescending.ToString(),
                    ["filterDetails"] = filterDetails ?? string.Empty,
                    ["externalFilter"] = externalFilter ?? string.Empty 
                };

                // Convert to query string
                var queryString = string.Join("&", query
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

                // Make GET request
                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Dashboard/GetGridUsermasterV2?{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<GenericGridModel<UserMasterVM>>>(content);

                if (result?.Data == null || !result.Data.ItemDetails.Any())
                {
                    return Json(new { success = true, data = new List<UserMasterVM>() });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}

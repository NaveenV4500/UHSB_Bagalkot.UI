using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels; 

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
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
                var response = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Category/GetGridallCategoryV2?{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"Error fetching content: {response.StatusCode}" });
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<GenericGridModel<CategoryVM>>>(content);

                if (result?.Data == null || !result.Data.ItemDetails.Any())
                {
                    return Json(new { success = true, data = new List<CategoryVM>() });
                }

                return Json(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return JSON
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Category(CategoryVM obj, [FromServices] IWebHostEnvironment env)
        {
            
         
                if (obj.ImageFile != null && obj.ImageFile.Length > 0)
                {
                    var uploadPath = _apiSettings.UploadSettings.UHSBPath;

                    if (!Directory.Exists(uploadPath+ "HorticultureHandbook/Category"))
                        Directory.CreateDirectory(uploadPath);
                     
                    var extension = Path.GetExtension(obj.ImageFile.FileName);
                    var fileName = $"{ obj.Name +"_"+DateTime.Now.ToString("dd-mm-yyyy") }{extension}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await obj.ImageFile.CopyToAsync(stream);
                    }
                     
                    obj.ImageUrl = Path.Combine("UHSBImageFiles", "HorticultureHandbook","Category", fileName).Replace("\\", "/");
            }

            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
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

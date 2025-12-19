using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UHSB_Bagalkot.Service.Common;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    //[Authorize]
    public class CommonApiController : Controller
    {
        private readonly ApiSettings _apiSettings;

        private readonly HttpClient _httpClient;

        public CommonApiController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiSettings = apiSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForwardApiResponse(string action_name = "")
        {
            var apiUrl = $"{_apiSettings.Base_Url}/{action_name}";

            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");


            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = _httpClient.GetAsync(apiUrl).Result;

            // If token expired, refresh and retry
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
               
                if (string.IsNullOrEmpty(refreshToken))
                    return RedirectToAction("Logout", "Account");

                var refreshedTokens = RefreshAccessToken(refreshToken);
                if (refreshedTokens == null)
                    return RedirectToAction("Logout", "Account");

                accessToken = HttpContext.Session.GetString("AccessToken");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                response = _httpClient.GetAsync(apiUrl).Result;
            }

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, $"Error calling API: {apiUrl}");

            var result = response.Content.ReadAsStringAsync().Result;
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

            return Content(result, contentType);
        }

         
        private ActionResult RefreshAccessToken(string refreshToken)
        {
            var refreshUrl = $"{_apiSettings.Base_Url}/refresh";  

            var data = new { RefreshToken = refreshToken };
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var refreshResponse = _httpClient.PostAsync(refreshUrl, content).Result;
            if (!refreshResponse.IsSuccessStatusCode)
                return null;

            var responseStr = refreshResponse.Content.ReadAsStringAsync().Result;
            dynamic tokenObj = JsonConvert.DeserializeObject(responseStr);
            string newAccessToken = tokenObj?.AccessToken?.ToString();
            string newRefreshToken = tokenObj?.RefreshToken?.ToString();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Remove("RefreshToken");
            HttpContext.Session.SetString("AccessToken", newAccessToken);
            HttpContext.Session.SetString("RefreshToken", newRefreshToken);
            if (string.IsNullOrEmpty(newAccessToken) || string.IsNullOrEmpty(newRefreshToken))
                return RedirectToAction("Logout", "Account");


            return null;
        }


    }
}

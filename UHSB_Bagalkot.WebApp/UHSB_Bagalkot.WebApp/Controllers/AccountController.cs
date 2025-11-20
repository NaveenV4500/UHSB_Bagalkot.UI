using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.ViewModels; 

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration config, IOptions<ApiSettings> apiSettings)
        {

            _httpClient = httpClientFactory.CreateClient("ApiClient");

            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            var loginData = new { phoneNumber = obj.PhoneNumber, UserName = obj.UserName, IsFromadmin = true };
            var jsonContent = JsonConvert.SerializeObject(loginData);
           
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiSettings.Base_Url}/Account/LoginLog", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid credentials";
                return View(obj);
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

            // Store tokens and user info in session
            HttpContext.Session.SetString("AccessToken", loginResponse.accessToken);
            HttpContext.Session.SetString("RefreshToken", loginResponse.refreshToken);
            HttpContext.Session.SetString("UserName", loginResponse.userName);
            HttpContext.Session.SetInt32("UserID", loginResponse.userID);  
            HttpContext.Session.SetInt32("userRoleType", loginResponse.userID); 

            // Optionally sign in using cookies if you want authentication
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResponse.userName),
            new Claim("UserID", loginResponse.userID.ToString())
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("AdminHome", "Dashboard");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //[UserMasterFilter]
        //[Authorize]
        //[SessionExpireFilter]
        public async Task<ActionResult> Register()
        {
            var userMaster = new UserMasterVM();

            #region ShowHideLogic
            var roleString = HttpContext.Session.GetString("userRoleType");

            if (!int.TryParse(roleString, out int roleValue))
            { 
                roleValue = (int)CommonEnum.UserRoleType.Select; // default
            }

            var role = (int)CommonEnum.UserRoleType.Admin;
            ViewBag.Role = role;

            ViewBag.CanAddEdit = role == (int)CommonEnum.UserRoleType.Admin ||
                                 role == (int)CommonEnum.UserRoleType.Scientist;
            #endregion

            // ===== Call API with token =====
            var token = HttpContext.Session.GetString("AuthToken");
          
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            // Get Roles
            var rolesResponse = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Account/GetAllUserRoleTypeAsDictionary");
             
            var roles = new Dictionary<int, string>();
            if (rolesResponse.IsSuccessStatusCode)
            {
                var json = await rolesResponse.Content.ReadAsStringAsync();
                roles = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(json);
            }
            userMaster.RoleTypeList= roles;

            // Get Districts
            var districtsResponse = await _httpClient.GetAsync($"{_apiSettings.Base_Url}/Account/GetAllDistrictsTypeAsDictionary");
             var districts = new Dictionary<int, string>();
            if (districtsResponse.IsSuccessStatusCode)
            {
                var json = await districtsResponse.Content.ReadAsStringAsync();
                districts = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, string>>(json);
            }
            userMaster.DistrictsList = districts;

            return View(userMaster);
        }
         
        [HttpPost]
        public async Task<ActionResult> RefreshToken()
        {
         
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            if (string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("Login");
            }
 
           
            var loginData = new { refreshToken = refreshToken };
            var jsonContent = JsonConvert.SerializeObject(loginData);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
             var response = await _httpClient.PostAsync($"{_apiSettings.Base_Url}/Account/refresh", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonString);
                 
                HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
                HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);

                return Json(new { success = true, message = "Token refreshed" });
            }
            else
            {
              
                return RedirectToAction("Login");
            }
        }

    }
}

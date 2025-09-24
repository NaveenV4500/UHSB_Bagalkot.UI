using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using UHSB_Bagalkot.WebService.AppSettings;
using UHSB_Bagalkot.WebService.ViewModels;

namespace UHSB_Bagalkot.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiSettings _apiSettings;

        public AccountController(IOptions<ApiSettings> apiSettings)
        {
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
            using var client = new HttpClient();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiSettings.Base_Url}/Account/login", content);

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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

    }
}

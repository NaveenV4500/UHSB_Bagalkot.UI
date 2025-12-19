using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
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
            ViewBag.OtpEnable = _apiSettings.OtpEnable;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginVM obj)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid login data"
                });
            }

            var loginData = new
            {
                phoneNumber = obj.PhoneNumber,
                UserName = obj.UserName,
                IsFromadmin = true
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{_apiSettings.Base_Url}/Account/LoginLog",
                content
            );

            // ❌ API FAILED
            if (!response.IsSuccessStatusCode)
            {
                var apiError = await response.Content.ReadAsStringAsync();

                return Json(new
                {
                    success = false,
                    message = string.IsNullOrWhiteSpace(apiError)
                        ? "Invalid credentials"
                        : apiError
                });
            }

            // ✅ API SUCCESS
            var jsonString = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

            // Store session values
            HttpContext.Session.SetString("AccessToken", loginResponse.accessToken);
            HttpContext.Session.SetString("RefreshToken", loginResponse.refreshToken);
            HttpContext.Session.SetString("UserName", loginResponse.userName);
            HttpContext.Session.SetString("UserID", loginResponse.userID.ToString());
            HttpContext.Session.SetString("userRoleType", loginResponse.userRoleType.ToString());
            HttpContext.Session.SetString("usercount", loginResponse.userCount.ToString());

            string role = loginResponse.userRoleType == "1" ? "admin" : "farmer";

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, loginResponse.userName),
        new Claim("UserID", loginResponse.userID.ToString()),
        new Claim(ClaimTypes.Role, role)
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("AdminHome", "Dashboard")
            });
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("AdminHome", "dashboard");
        }

        //[UserMasterFilter]
        //[Authorize]
        //[SessionExpireFilter]

        [AllowAnonymous]
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
            userMaster.RoleTypeList = roles;

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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAutoLogin([FromBody] UserMasterVM model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "Invalid data"
            //    });
            //}

            var token = HttpContext.Session.GetString("AuthToken");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{_apiSettings.Base_Url}/Account/registernew",
                jsonContent
            );

            // ✅ API SUCCESS
            if (response.IsSuccessStatusCode)
            {
                return Json(new
                {
                    success = true,
                    message = "User registered successfully"
                });
            }

            // ❌ API FAILED
            var error = await response.Content.ReadAsStringAsync();

            return Json(new
            {
                success = false,
                message = string.IsNullOrWhiteSpace(error)
                    ? "Registration failed"
                    : error
            });
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<JsonResult> DeleteUser([FromBody] int userid)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Session expired" });
            }

            try
            {
                using var httpClient = new HttpClient();

                // API URL with query string
                var apiUrl = _apiSettings.Base_Url + $"/Account/deleteuser?userid={userid}";

                var response = await httpClient.PostAsync(apiUrl, null); // no body needed
                var responsetext = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Delete failed", details = responsetext });
                }

                return Json(new { success = responsetext });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SendOtp([FromBody] LoginVM obj)
        {
            try
            {
                CommonEnum.WriteLog($"SendOtp request received for email: {obj.UserName}");

                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiSettings.Base_Url}/Account/SendOtp", content);
                CommonEnum.WriteLog($"SendOtp API response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    CommonEnum.WriteLog($"Unable to send OTP for email: {obj.UserName}");
                    return Json(new OtpResponse
                    {
                        Success = false,
                        Message = "Unable to send OTP."
                    });
                }

                var result = await response.Content.ReadAsStringAsync();
                CommonEnum.WriteLog($"SendOtp API response: {result}");

                var apiRes = JsonConvert.DeserializeObject<OtpResponse>(result);
                apiRes.Email = apiRes.Email?.Trim();

                return Json(apiRes);
            }
            catch (Exception ex)
            {
                CommonEnum.WriteLog($"Exception in SendOtp for email: {obj.UserName} - {ex.Message}");
                return Json(new OtpResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtpPost([FromBody] VerifyOtpVM obj)
        {
            try
            {
                CommonEnum.WriteLog($"VerifyOtpPost request received for UserID: {obj.UserId}");

                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiSettings.Base_Url}/Account/VerifyOtpPost", content);

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "OTP verification failed" });
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

                if (!loginResponse.success)
                {
                    return Json(new { success = false, message = loginResponse.userName });
                }

                HttpContext.Session.SetString("AccessToken", loginResponse.accessToken);
                HttpContext.Session.SetString("RefreshToken", loginResponse.refreshToken);
                HttpContext.Session.SetString("UserName", loginResponse.userName);
                HttpContext.Session.SetInt32("UserID", loginResponse.userID);
                HttpContext.Session.SetInt32("userRoleType", Convert.ToInt16(loginResponse.userRoleType));
                string role = loginResponse.userRoleType == "1"
                           ? "admin"
                           : "farmer";
                // Optionally sign in using cookies if you want authentication
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResponse.userName),
            new Claim("UserID", loginResponse.userID.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Json(new { success = true, redirectUrl = Url.Action("AdminHome", "Dashboard") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


         
    }
}

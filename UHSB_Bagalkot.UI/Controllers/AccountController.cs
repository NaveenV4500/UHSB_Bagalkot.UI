using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.AppSettings;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.UI.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly HttpClient _httpClient = new HttpClient(); 
        private readonly IEmailService _emailService;
        public AccountController(IAccountRepository accountRepository, IConfiguration configuration, TokenService tokenService,IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailService = emailService;
        }



        [HttpPost("LoginLog")]
        public async Task<IActionResult> LoginLog([FromBody] LoginVM request)
        {
            CommonEnum.WriteLog($"Login attempt for phone: {request.PhoneNumber}");

            try
            {
                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    CommonEnum.WriteLog("Login failed: Phone number is empty.");
                    return new JsonResult(new { success = false, message = "Phone number is required." }) { StatusCode = 401 };
                }

                request.PhoneNumber = request.PhoneNumber.Trim('"');

                var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber, request.UserName, request.IsFromadmin);

                if (user == null)
                {
                    CommonEnum.WriteLog($"Login failed: Phone number {request.PhoneNumber} not registered or inactive.");
                    return new JsonResult(new { success = false, message = "Phone number not registered or inactive." }) { StatusCode = 401 };
                }
                if (!user.IsActive && (user.RoleType == (byte)CommonEnum.UserRoleType.Scientist))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Registered successfully. Pending for admin approval."
                    })
                    { StatusCode = 200 };  
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                };

                var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
                var refreshToken = _tokenService.GenerateRefreshToken();
                _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);

                var count = _accountRepository.GetUsersCount();
                CommonEnum.WriteLog($"Login successful for user: {user.UserName}, UserID: {user.Id}");

                return Ok(new
                {
                    success = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    UserName = user.UserName,
                    userRoleType = user.RoleType,
                    phoneNo = user.PhoneNumber,
                    UserID = user.Id,
                    UserCount = count,
                    isactive=user.IsActive
                });
            }
            catch (Exception ex)
            {

                var errorMessage = $"Login error for phone: {request.PhoneNumber}.\n" +
                                   $"Exception: {ex.Message}\n" +
                                   $"Inner Exception: {ex.InnerException?.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}";

                CommonEnum.WriteLog(errorMessage);

                return new JsonResult(new
                {
                    success = false,
                    message = errorMessage
                })
                { StatusCode = 500 };
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM request)
        {
            CommonEnum.WriteLog($"Login attempt for phone: {request.PhoneNumber}");

            try
            {
                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    CommonEnum.WriteLog("Login failed: Phone number is empty.");
                    return new JsonResult(new { success = false, message = "Phone number is required." }) { StatusCode = 401 };
                }

                request.PhoneNumber = request.PhoneNumber.Trim('"');

                var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber, request.UserName, request.IsFromadmin);

                if (user == null)
                {
                    CommonEnum.WriteLog($"Login failed: Phone number {request.PhoneNumber} not registered or inactive.");
                    return new JsonResult(new { success = false, message = "Phone number not registered or inactive." }) { StatusCode = 401 };
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                };

                var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
                var refreshToken = _tokenService.GenerateRefreshToken();
                _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);

                var count = _accountRepository.GetUsersCount();
                CommonEnum.WriteLog($"Login successful for user: {user.UserName}, UserID: {user.Id}");

                return Ok(new
                {
                    success = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    UserName = user.UserName,
                    userRoleType = user.RoleType,
                    phoneNo = user.PhoneNumber,
                    UserID = user.Id,
                    UserCount = count
                });
            }
            catch (Exception ex)
            {

                var errorMessage = $"Login error for phone: {request.PhoneNumber}.\n" +
                                   $"Exception: {ex.Message}\n" +
                                   $"Inner Exception: {ex.InnerException?.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}";

                CommonEnum.WriteLog(errorMessage);

                return new JsonResult(new
                {
                    success = false,
                    message = errorMessage
                })
                { StatusCode = 500 };
            }

        }


        [HttpPost("loginOriginal")]
        public async Task<IActionResult> loginOriginal([FromBody] LoginVM request)
        {
            try
            {

                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    return new JsonResult(new { success = false, message = "Phone number is required." }) { StatusCode = 401 };
                }

                request.PhoneNumber = request.PhoneNumber.Trim('"');

                var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber, request.UserName, request.IsFromadmin);

                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Phone number not registered or inactive." }) { StatusCode = 401 };
                }


                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                };

                var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
                var refreshToken = _tokenService.GenerateRefreshToken();


                _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);
                var count = _accountRepository.GetUsersCount();

                return Ok(new
                {
                    success = true,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    UserName = user.UserName,
                    userRoleType = user.RoleType,
                    phoneNo = user.PhoneNumber,
                    UserID = user.Id,
                    UserCount = count
                });
            }
            catch (Exception ex)
            {
                CommonEnum.WriteLog($"Login error for phone: {request.PhoneNumber}. Exception: {ex.Message}");
                return new JsonResult(new { success = false, message = "An unexpected error occurred \n ." + ex.Message + "\n Inner msg : " + ex.InnerException }) { StatusCode = 500 };
            }
        }


        [HttpPost("loginOld")]
        public async Task<IActionResult> LoginOld([FromBody] LoginVM request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber))
                return new JsonResult(new { success = false, message = "Phone number is required." }) { StatusCode = 401 };


            request.PhoneNumber = request.PhoneNumber.Trim('"');

            var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);

            if (user == null)
                return new JsonResult(new { success = false, message = "Phone number not registered or inactive." }) { StatusCode = 401 };


            // build claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
        // new Claim(ClaimTypes.Role, user.RoleType)
    };

            // generate tokens
            var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
            var refreshToken = _tokenService.GenerateRefreshToken();

            // save refresh token in DB
            _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);
            var count = _accountRepository.GetUsersCount();

            return Ok(new
            {
                success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserName = user.UserName,
                userRoleType = user.RoleType.ToString(),
                phoneNo = user.PhoneNumber,
                UserID = user.Id,
                UserCount = count
            });
        }

        [HttpPost("registernew")]
        public async Task<IActionResult> RegisterNew([FromBody] UserMasterRequestmobile request)
        {
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  


            if(request.RoleType == (byte)CommonEnum.UserRoleType.Select)
                return BadRequest(new { success = false, message = "Please select a valid role type." });

            if (request.RoleType == (byte)CommonEnum.UserRoleType.Scientist)
            {
                if (!request.EmployeeId.HasValue || request.EmployeeId.Value.ToString().Length != 5)
                    return BadRequest(new { success = false, message = "Please provide a valid Employee ID for Scientist role." });

                if (string.IsNullOrEmpty(request.EmailId))
                {
                    return BadRequest(new { success = false, message = "Email ID is required for Scientist role." });
                }

                request.IsActive = false;
                msg= "Registered successfully !. Pending for admin approval.";
            }
            else
            {
                msg= "Registration successful!";
            }

            var existingUser = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);
            if (existingUser != null)
                return Conflict(new { success = false, message = "User already exists with this phone number." });

            request.PasswordHash = ComputeSha256Hash(request.UserName);

            var result = await _accountRepository.CreateOrUpdateUserAsync(request);
            if (!result)
                return StatusCode(500, new { success = false, message = "Failed to register user." });

            return Ok(new { success = true, message = msg });
        }


        [HttpPost("registerold")]
        public async Task<IActionResult> Register([FromBody] UserMasterVM request)
        {
            if (string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.PhoneNumber))
            {
                return new JsonResult(new { success = false, message = "All fields are required." }) { StatusCode = 400 };
            }
            var existingUser = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);
            if (existingUser != null)
                return new JsonResult(new { success = false, message = "Phone number already registered." }) { StatusCode = 400 };

            // Hash the password
            var passwordHash = ComputeSha256Hash(request.UserName);
            request.PasswordHash = passwordHash;
            // Create user


            //await _accountRepository.CreateUserAsync(request);

            return Ok(new { Message = "User registered successfully." });
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                var builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshVM model)
        {
            var storedToken = _tokenService.GetRefreshTokenFromDb(model.RefreshToken);

            if (storedToken == null)
                return Unauthorized("Invalid or expired refresh token");

            // generate new access + refresh tokens
            var newAccessToken = _tokenService.GenerateAccessToken(new[]
            {
        new Claim(ClaimTypes.Name, "username") // ideally from storedToken.UserId
    });

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // revoke old token
            storedToken.Revoked = DateTime.UtcNow;

            // save new token
            _tokenService.SaveRefreshTokenToDb(storedToken.UserId, newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        [HttpGet("GetAllUserRoleTypeAsDictionary")]
        public async Task<IActionResult> GetAllUserRoleTypeAsDictionary()
        {
            var data = await Task.Run(() => _accountRepository.GetAllUserRoleTypeAsDictionary());
            return Ok(data);
        }

        [HttpGet("GetAllDistrictsTypeAsDictionary")]
        public async Task<IActionResult> GetAllDistrictsTypeAsDictionary()
        {
            var data = await Task.Run(() => _accountRepository.GetAllDistrictsTypeAsDictionary());
            return Ok(data);
        }


        #region profile update and get 
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile(string phone)
        {
            var profile = await _accountRepository.GetUserByPhoneAsync(phone);
            if (profile == null)
                return NotFound(new { success = false, message = "Profile not found" });

            return Ok(profile);
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserMasterRequestmobile profile)
        {
            if (profile == null || string.IsNullOrEmpty(profile.PhoneNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            var result = await _accountRepository.CreateOrUpdateUserAsync(profile);
            if (!result)
                return BadRequest(new { success = false, message = "Failed to update profile" });

            return Ok(new { success = true, message = "Profile updated successfully" });
        }
        #endregion

        #region Delete Userid
        [HttpPost("deleteuser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int userid)
        {
            string errorMessage = string.Empty;
            var data = await Task.Run(() => _accountRepository.DeleteUser(userid, out errorMessage));
            return Ok(errorMessage);
        }



        #endregion



        #region OTP Master
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] LoginVM obj)
        {
            try
            {
                CommonEnum.WriteLog($"SendOtp request received: UserName={obj.UserName}, PhoneNumber={obj.PhoneNumber}");

                if (string.IsNullOrWhiteSpace(obj.UserName) || string.IsNullOrWhiteSpace(obj.PhoneNumber))
                {
                    CommonEnum.WriteLog("SendOtp validation failed: Username or Phone Number missing.");
                    return BadRequest(new { success = false, message = "Username and Phone Number are required." });
                }

                obj.UserName = obj.UserName.Trim();
                obj.PhoneNumber = obj.PhoneNumber.Trim();

                var user = await _accountRepository.GetUserByPhoneAsync(obj.PhoneNumber, obj.UserName, obj.IsFromadmin);
                if (user == null)
                {
                    CommonEnum.WriteLog($"SendOtp failed: User not found or inactive for PhoneNumber={obj.PhoneNumber}");
                    return Ok(new { success = false, message = "User not found or inactive." });
                }

                int otp = new Random().Next(100000, 999999);
                CommonEnum.WriteLog($"Generated OTP {otp} for UserID={user.Id}");

                await _accountRepository.SaveOtpAsync(user.Id, otp);
                CommonEnum.WriteLog($"Saved OTP for UserID={user.Id}");

                await _emailService.SendEmailAsync(
                    user.EmailId,
                    "UHSB Horti Guide Login OTP",
                    $"Hello {user.UserName},\n\nYour OTP for login is {otp}. It is valid for 5 minutes.\n\nRegards,\nUHSB Bagalkot"
                );
                CommonEnum.WriteLog($"OTP email sent to {user.EmailId} for UserID={user.Id}");

                return Ok(new { success = true, userId = user.Id, email = user.EmailId, message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                CommonEnum.WriteLog($"Exception in SendOtp: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }


        [HttpPost("VerifyOtpPost")]
        public async Task<IActionResult> VerifyOtpPost([FromBody] VerifyOtpVM obj)
        {
            if (obj.UserId <= 0 || string.IsNullOrWhiteSpace(obj.Otp))
                return BadRequest(new { success = false, message = "Invalid request." });

            var otpRecord = await _accountRepository.GetLatestOtpAsync(obj.UserId);
            if (otpRecord == null)
                return Ok(new { success = false, message = "OTP not found. Please request again." });

            if (otpRecord.IsUsed)
                return Ok(new { success = false, message = "OTP already used. Request a new OTP." });

            if (otpRecord.ExpiryTime < DateTime.Now)
                return Ok(new { success = false, message = "OTP expired. Request a new OTP." });

            if (otpRecord.Otp.ToString() != obj.Otp)
                return Ok(new { success = false, message = "Invalid OTP." });

            // Mark OTP as used
            otpRecord.IsUsed = true;
            await _accountRepository.UpdateOtpAsync(otpRecord);

            // ⬇️ Prepare login request payload for internal login API call
            var loginRequest = new LoginVM
            {
                PhoneNumber = obj.PhoneNumber,
                UserName = obj.UserName,
                IsFromadmin = true
            };

            // 🔥 Instead of HTTP call, invoke login API logic directly
            var loginResult = await LoginInternal(loginRequest);
            if (!loginResult.success)
                return Ok(loginResult);

            return Ok(loginResult);
        }
        private async Task<dynamic> LoginInternal(LoginVM request)
        {
            var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber, request.UserName, request.IsFromadmin);
            if (user == null)
                return new { success = false, message = "Phone number not registered or inactive." };

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
    };

            var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);

            return new
            {
                success = true,
                accessToken = accessToken,
                refreshToken = refreshToken.Token,
                userName = user.UserName,
                userId = user.Id,
                userRoleType = user.RoleType,
                phoneNo = user.PhoneNumber,
                isactive = user.IsActive
            };
        }

        #endregion
    }
}
 

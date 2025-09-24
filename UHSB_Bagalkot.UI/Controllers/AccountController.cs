using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using UHSB_Bagalkot.Data;

using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels;
using UHSB_Bagalkot.UI.Common;

namespace UHSB_Bagalkot.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController  : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        public AccountController(IAccountRepository accountRepository, IConfiguration configuration, TokenService tokenService)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM request)
        {
            try
            {
              CommonEnum.WriteLog($"[Login] Incoming request: PhoneNumber = {request.PhoneNumber}");

                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    CommonEnum.WriteLog("[Login] Phone number is empty or null");
                    return new JsonResult(new { success = false, message = "Phone number is required." }) { StatusCode = 401 };
                }

                request.PhoneNumber = request.PhoneNumber.Trim('"');
                CommonEnum.WriteLog($"[Login] Trimmed PhoneNumber: {request.PhoneNumber}");

                var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber,request.UserName,request.IsFromadmin);

                if (user == null)
                {
                    CommonEnum.WriteLog($"[Login] No user found for PhoneNumber: {request.PhoneNumber}");
                    return new JsonResult(new { success = false, message = "Phone number not registered or inactive." }) { StatusCode = 401 };
                }

                CommonEnum.WriteLog($"[Login] User found: UserName = {user.UserName}, UserId = {user.Id}, Role = {user.RoleType}");

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
        };

                var accessToken = _tokenService.GenerateAccessToken(claims.ToArray());
                var refreshToken = _tokenService.GenerateRefreshToken();

                CommonEnum.WriteLog("[Login] Tokens generated successfully");

                _tokenService.SaveRefreshTokenToDb(user.Id, refreshToken);
                CommonEnum.WriteLog($"[Login] Refresh token saved to DB for UserId: {user.Id}");
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
            catch (Exception ex)
            {
                CommonEnum.WriteLog($"[Login] Exception: {ex.Message}");
                return new JsonResult(new { success = false, message = "An unexpected error occurred." + ex.Message }) { StatusCode = 500 };
            }
        }


        [HttpPost("loginOld")]
        public async Task<IActionResult> LoginOld([FromBody] LoginVM request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber))
                return new JsonResult(new { success = false,message = "Phone number is required." }) { StatusCode = 401 };


            request.PhoneNumber = request.PhoneNumber.Trim('"');

            var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);

            if (user == null)
                return new JsonResult(new { success = false,message = "Phone number not registered or inactive." }) { StatusCode = 401 };
             

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
                phoneNo=user.PhoneNumber,
                UserID=user.Id,
                UserCount=count
            });
        }


        [HttpPost("register")]
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
            request.PasswordHash= passwordHash;
            // Create user


            await _accountRepository.CreateUserAsync(request);

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
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            var storedToken = _tokenService.GetRefreshTokenFromDb(refreshToken);

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

    }

}

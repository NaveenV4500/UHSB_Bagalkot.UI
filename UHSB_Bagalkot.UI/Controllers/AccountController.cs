using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UHSB_Bagalkot.Data;
using UHSB_Bagalkot.Data.DataAccess;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.Repositories;
using UHSB_Bagalkot.Service.ViewModels;

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
            if (string.IsNullOrEmpty(request.PhoneNumber))
                return BadRequest("Phone number is required.");

            var user = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);

            if (user == null)
                return Unauthorized("Phone number not registered or inactive.");

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

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserName = user.UserName,
                Roles = user.RoleType
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserMaster request)
        {
            if (string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.PhoneNumber))
            {
                return BadRequest("All fields are required.");
            }

            // Check if phone number already exists
            var existingUser = await _accountRepository.GetUserByPhoneAsync(request.PhoneNumber);
            if (existingUser != null)
                return BadRequest("Phone number already registered.");

            // Hash the password
            var passwordHash = ComputeSha256Hash(request.UserName);

            // Create user
            var user = new UserMaster
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                RoleType = request.RoleType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepository.CreateUserAsync(user);

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

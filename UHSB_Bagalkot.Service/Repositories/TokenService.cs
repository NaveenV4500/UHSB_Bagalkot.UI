using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data;

using UHSB_Bagalkot.Service.ViewModels;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class TokenService:CommonConnection
    {
        private readonly IConfiguration _configuration;

        public TokenService(Uhsb2025Context context, IConfiguration configuration)
            : base(context)   
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(Claim[] claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),   
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        public void SaveRefreshTokenToDb(int userId, RefreshToken refreshToken)
        {
           
                var oldTokens = _context.RefreshTokens.Where(r => r.UserId == userId).ToList();
                foreach (var t in oldTokens)
                {
                    t.Revoked = DateTime.UtcNow;
                }

                // Save new refresh token
                refreshToken.UserId = userId;
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();
           
        }


        public RefreshToken GetRefreshTokenFromDb(string token)
        {
            
                return _context.RefreshTokens
                              .FirstOrDefault(r => r.Token == token && r.Revoked == null && r.Expires > DateTime.UtcNow);
           
        }


    }
}

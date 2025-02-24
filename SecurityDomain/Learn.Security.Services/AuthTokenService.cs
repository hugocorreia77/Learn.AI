using Learn.Security.Services.Abstractions.Interfaces;
using Learn.Users.Repository.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Learn.Security.Services
{
    public class AuthTokenService(IConfiguration config) : IAuthTokenService
    {
        private readonly IConfiguration _config = config;
        private readonly double DefaultExpirationTime = 30;

        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            string secret = jwtSettings?["Secret"] ?? string.Empty;
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("JWT Secret settings not found.");
            }
            string issuer = jwtSettings?["Issuer"] ?? string.Empty;
            if (string.IsNullOrEmpty(issuer))
            {
                throw new Exception("JWT Issuer settings not found.");
            }
            string audience = jwtSettings?["Audience"] ?? string.Empty;
            if (string.IsNullOrEmpty(issuer))
            {
                throw new Exception("JWT Audience settings not found.");
            }
            double expiration = DefaultExpirationTime;
            if (!string.IsNullOrEmpty(jwtSettings?["ExpireMinutes"]))
            {
                expiration = Convert.ToDouble(jwtSettings["ExpireMinutes"]);
            }


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiration)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using Learn.Core.Shared.Models.User;
using Learn.Core.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Learn.Core.Shared.Services
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly string DefaultLanguage = "pt-PT";

        public string GetSelectedLanguage()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.FirstOrDefault() ?? DefaultLanguage; 
        }

        #region public UserReference GetUser()
        public UserReference GetUser()
        {
            var claimId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var claimName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            var claimUsername = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            if (string.IsNullOrEmpty(claimId))
            {
                throw new Exception("JWT token is invalid. Id not found.");
            }
            if (!Guid.TryParse(claimId, out var userId))
            {
                throw new Exception("JWT token is invalid. Id can not be parsed.");
            }
            if (string.IsNullOrEmpty(claimName))
            {
                throw new Exception("JWT token is invalid. Name not found.");
            }
            if (string.IsNullOrEmpty(claimUsername))
            {
                throw new Exception("JWT token is invalid. Username not found.");
            }

            return new UserReference
            {
                Id = userId,
                Name = claimName,
                Username = claimName
            };
        }
        #endregion
    }
}

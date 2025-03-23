using Learn.Users.Models;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace Learn.Core.Shared.Services
{
    public static class UserHubContextExtensions
    {
        public static UserReference GetUser(this HubCallerContext hubContext)
        {
            var claimId = hubContext.User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var claimName = hubContext.User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            var claimUsername = hubContext.User?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

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
                Username = claimUsername
            };
        }
    }
}

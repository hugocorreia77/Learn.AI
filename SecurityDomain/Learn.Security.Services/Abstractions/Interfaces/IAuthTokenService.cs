using Learn.Users.Repository.Models;

namespace Learn.Security.Services.Abstractions.Interfaces
{
    public interface IAuthTokenService
    {
        string GenerateToken(User user);
    }
}

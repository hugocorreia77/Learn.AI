using Learn.Core.Shared.Security.Abstractions;

namespace Learn.Core.Shared.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

namespace Learn.Core.Shared.Security.Abstractions
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}

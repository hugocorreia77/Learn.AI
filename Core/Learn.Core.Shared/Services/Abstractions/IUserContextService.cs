using Learn.Users.Models;

namespace Learn.Core.Shared.Services.Abstractions
{
    public interface IUserContextService
    {
        UserReference GetUser();
        string GetSelectedLanguage();
    }
}

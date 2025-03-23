using Learn.Users.Models;

namespace Learn.Core.Shared.Services.Abstractions
{
    public interface IUserHubContextService
    {
        UserReference GetUser();
    }
}

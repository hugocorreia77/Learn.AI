using Learn.Core.Shared.Models.User;

namespace Learn.Core.Shared.Services.Abstractions
{
    public interface IUserHubContextService
    {
        UserReference GetUser();
    }
}

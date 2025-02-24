using Learn.Core.Shared.Models.Response;
using Learn.Users.Models.Input;
using Learn.Users.Repository.Models;

namespace Learn.Users.Services.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<BaseContentResponse<User>> CreateUserAsync(CreateUserInput user, CancellationToken cancellationToken);
    }
}

using Learn.Core.Shared.Models.Response;
using Learn.Users.Repository.Models;

namespace Learn.Users.Repository.Repositories
{
    public interface IUserRepository
    {
        Task<BaseContentResponse<User>> CreateUserAsync(User model,  CancellationToken cancellationToken);
        Task<BaseContentResponse<User>> GetUserAsync(string username, string password,  CancellationToken cancellationToken);
        Task<BaseContentResponse<User>> GetUserAsync(string username, CancellationToken cancellationToken);
    }
}

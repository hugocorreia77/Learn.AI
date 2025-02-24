using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Security.Abstractions;
using Learn.Users.Models.Input;
using Learn.Users.Repository.Models;
using Learn.Users.Repository.Repositories;
using Learn.Users.Services.Abstractions.Interfaces;

namespace Learn.Users.Services
{
    public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
    {
        #region public async Task<BaseContentResponse<User>> CreateUserAsync(CreateUserInput user, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<User>> CreateUserAsync(CreateUserInput user, CancellationToken cancellationToken)
        {
            var userToCreate = new User
            {
                Name = user.Name,
                Username = user.Username,
                Password = passwordHasher.HashPassword(user.Password),
                CountryCode = user.CountryCode,
            };

            return await userRepository.CreateUserAsync(userToCreate, cancellationToken);
        }
        #endregion
    }
}

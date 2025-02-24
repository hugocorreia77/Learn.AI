using Learn.Core.Shared.Extensions;
using Learn.Core.Shared.Models.Response;
using Learn.Core.Shared.Security.Abstractions;
using Learn.Security.Services.Abstractions.Interfaces;
using Learn.Users.Repository.Repositories;

namespace Learn.Security.Services
{
    public class SecurityService(IUserRepository userRepository
        , IAuthTokenService authTokenService)
        : ISecurityService
    {

        #region public async Task<BaseContentResponse<string>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        public async Task<BaseContentResponse<string>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var userResult = await userRepository.GetUserAsync(username, cancellationToken);
            if (!userResult.Success)
            {
                return new BaseContentResponse<string>
                {
                    Errors = userResult.Errors,
                    Warnings = userResult.Warnings,                    
                }.SetFailed();
            }
            if(userResult.Data is null)
            {
                return new BaseContentResponse<string>().SetFailed().AddError("User not found.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, userResult.Data.Password);
            if (!isPasswordValid)
            {
                return new BaseContentResponse<string>().SetFailed().AddError("User not found.");
            }

            var token = authTokenService.GenerateToken(userResult.Data);
            return new BaseContentResponse<string>
            {
                Data = token,
            }.SetSucceeded();
        }
        #endregion

    }
}

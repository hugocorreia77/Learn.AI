using Blazored.LocalStorage;
using Learn.Core.Shared.App.Authentication;
using Learn.Core.Shared.Models.Response;
using Learn.Security.App.Services.Abstractions;
using Learn.Security.Client.Abstractions;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Learn.Security.App.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Properties
        private readonly ILearnSecurityClient _learnSecurityClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly string AUTH_TOKEN = "authToken";
        #endregion

        #region ctor
        public AuthenticationService(ILearnSecurityClient learnSecurityClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider) {
            _learnSecurityClient = learnSecurityClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }
        #endregion

        #region AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        public async Task<BaseResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var response = await _learnSecurityClient.AuthenticateAsync(new Security.Models.Input.AuthenticationInput
            {
                Username = username,
                Password = password
            }, cancellationToken);

            var authStateProvider = _authenticationStateProvider as CustomAuthenticationStateProvider;
            if (response.Success)
            {
                // Armazena o token JWT no localStorage
                await _localStorage.SetItemAsync(AUTH_TOKEN, response.Data);
                // Notifica o AuthenticationStateProvider que o usuário está autenticado
                authStateProvider.MarkUserAsAuthenticated(response.Data);
            }
            else
            {
                authStateProvider.MarkUserAsLoggedOut();
            }

            return response;
        }
        #endregion

        #region LogoutAsync(CancellationToken cancellationToken = default)
        public async Task<BaseResponse> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var authStateProvider = _authenticationStateProvider as CustomAuthenticationStateProvider;
            var state = await authStateProvider.GetAuthenticationStateAsync();
            if (state != null && state.User != null)
            {
                authStateProvider.MarkUserAsLoggedOut();
                await _localStorage.RemoveItemAsync(AUTH_TOKEN);
            }

            return new BaseResponse();
        }
        #endregion

        #region CurrentUserNameAsync()
        public async Task<string> CurrentUserNameAsync()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();

            if(state.User.Identity?.Name is not null)
            {
                return state.User.Identity.Name;
            }

            var nameClaim = state.User.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
            if(nameClaim is not null)
            {
                return nameClaim.Value;
            }

            return string.Empty;
        }
        #endregion

    }
}

using Blazored.LocalStorage;
using Learn.Core.Shared.App.Authentication;
using Learn.Core.Shared.Models.Response;
using Learn.Security.App.Services.Abstractions;
using Learn.Security.Client.Abstractions;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Learn.Security.App.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILearnSecurityClient _learnSecurityClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(ILearnSecurityClient learnSecurityClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider) {
            _learnSecurityClient = learnSecurityClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<BaseResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var response = await _learnSecurityClient.AuthenticateAsync(new Models.Input.AuthenticationInput
            {
                Username = username,
                Password = password
            }, cancellationToken);

            if (response.Success)
            {
                // Armazena o token JWT no localStorage
                await _localStorage.SetItemAsync("authToken", response.Data);

                // Notifica o AuthenticationStateProvider que o usuário está autenticado
                var z = _authenticationStateProvider as CustomAuthenticationStateProvider;
                z.MarkUserAsAuthenticated(response.Data);
            }
            else
            {
                var z = _authenticationStateProvider as CustomAuthenticationStateProvider;
                z.MarkUserAsLoggedOut();
            }

            return response;
        }
    }
}

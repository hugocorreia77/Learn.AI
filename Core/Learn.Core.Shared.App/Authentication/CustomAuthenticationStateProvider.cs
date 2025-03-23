using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Learn.Core.Shared.App.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        #region Properties
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly string AuthTokenKey = "authToken";
        private readonly string AuthenticationType = "jwt";
        private readonly string AuthenticationScheme = "Bearer";
        #endregion

        #region Constructor
        public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }
        #endregion

        #region public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(AuthTokenKey);

            var jwt = GetSecurityToken(token ?? string.Empty);
            if (jwt.ValidTo > DateTime.Now)
            {
                var principalClaim = GetClaimPrincipal(jwt);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, token);
                return new AuthenticationState(principalClaim);
            }

            return new AuthenticationState(new ClaimsPrincipal());
        }
        #endregion

        #region public void MarkUserAsAuthenticated(string token)
        public void MarkUserAsAuthenticated(string token)
        {
            var jwt = GetSecurityToken(token);
            var user = GetClaimPrincipal(jwt);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        #endregion

        #region public void MarkUserAsLoggedOut()
        public void MarkUserAsLoggedOut()
        {
            _localStorage.RemoveItemAsync(AuthTokenKey);
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
        #endregion

        #region private ClaimsPrincipal GetClaimPrincipal(JwtSecurityToken jwt)
        private ClaimsPrincipal GetClaimPrincipal(JwtSecurityToken jwt)
        {
            if (jwt.ValidTo > DateTime.Now)
            {
                var identity = new ClaimsIdentity(jwt.Claims, AuthenticationType);
                return new ClaimsPrincipal(identity);
            }
            return new ClaimsPrincipal();
        }
        #endregion

        #region private JwtSecurityToken GetSecurityToken(string token)
        private static JwtSecurityToken GetSecurityToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new JwtSecurityToken();
            }
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
        #endregion

    }

}

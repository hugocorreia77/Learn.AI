using Learn.Core.Shared.App.Pages;
using Learn.Security.App.Models;
using Learn.Security.App.Services.Abstractions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Learn.Security.App.Pages
{

    public class AuthenticationPageViewModel : BaseComponentViewModel
    {
        #region Properties
        public LoginModel LoginModel { get; set; } = new LoginModel();
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationPageViewModel> _logger;
        #endregion

        #region ctor
        public AuthenticationPageViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _logger = serviceProvider.GetRequiredService<ILogger<AuthenticationPageViewModel>>();
            SetContext(LoginModel);
        }
        #endregion

        #region HandleLoginSubmission()
        public async Task HandleLoginSubmission()
        {
            IsBusy = true;
            try
            {
                MessageStore.Clear();

                if(EditContext.Validate())
                {
                    var response = await _authenticationService.AuthenticateAsync(LoginModel.Username, LoginModel.Password);

                    if (response.Success)
                    {
                        Navigation.NavigateTo("/");
                    }
                    else
                    {
                        var fi = new FieldIdentifier(LoginModel.Password, "Password");
                        MessageStore.Add(fi, "Login inválido.");
                        EditContext.NotifyValidationStateChanged();
                    }
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                _logger.LogError(ex, "An error ocurred while attempting to login: {ErrorMessage}", ex.Message);
            }
        }
        #endregion

    }
}

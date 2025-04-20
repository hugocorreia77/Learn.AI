using Learn.Core.Shared.App.Extensions;
using Learn.Security.App.Pages;
using Learn.Security.App.Services;
using Learn.Security.App.Services.Abstractions;
using Learn.Security.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learn.Security.App.StartupExtensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSecurityClient(configuration);
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.AddViewModelPages(System.Reflection.Assembly.GetAssembly(typeof(AuthenticationPageViewModel)));

            return services;
        }
    }
}

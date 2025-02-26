using Learn.Core.Shared.Security.Abstractions;
using Learn.Core.Shared.Services;
using Learn.Core.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Learn.Core.Shared.Security.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            return services;
        }

    }
}

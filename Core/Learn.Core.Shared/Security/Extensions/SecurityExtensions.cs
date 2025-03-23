using Learn.Core.Shared.Security.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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

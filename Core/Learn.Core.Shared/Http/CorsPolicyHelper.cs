using Microsoft.Extensions.DependencyInjection;

namespace Learn.Core.Shared.Http
{
    public static class CorsPolicyHelper
    {
        public static readonly string CorsPolicyName = "CorsPolicy";

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyHelper.CorsPolicyName, policy =>
                {
                    policy.AllowAnyOrigin() // ⚠️ Se precisar restringir, use .WithOrigins("https://seusite.com")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

    }
}

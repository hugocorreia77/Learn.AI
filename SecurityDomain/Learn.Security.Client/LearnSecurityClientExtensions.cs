using Learn.Core.Shared.Clients;
using Learn.Security.Client.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learn.Security.Client
{
    public static class LearnSecurityClientExtensions
    {
        public static IServiceCollection AddSecurityClient(this IServiceCollection services, IConfiguration configuration)
        {
            DomainHttpClientConfiguration? config = configuration.GetSection("KnownDomains").GetSection("Security").Get<DomainHttpClientConfiguration>();
            if (config is null)
            {
                throw new Exception("KnownDomains:Security http client configuration is missing.");
            }

            services.AddHttpContextAccessor();
            services.AddTransient<AuthHeaderHandler>();

            services.AddHttpClient<ILearnSecurityClient, LearnSecurityClient>(client =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var language = httpContextAccessor?.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault() ?? "pt-PT";

                client.BaseAddress = new Uri(config.Url);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Language", language);
            }).AddHttpMessageHandler<AuthHeaderHandler>();

            return services;
        }
    }
}

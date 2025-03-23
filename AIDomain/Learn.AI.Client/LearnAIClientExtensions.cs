using Learn.AI.Client.Abstractions;
using Learn.Core.Shared.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learn.AI.Client
{
    public static class LearnAIClientExtensions
    {

        public static IServiceCollection AddAIClient(this IServiceCollection services, IConfiguration configuration)
        {
            DomainHttpClientConfiguration? config = configuration.GetSection("KnownDomains").GetSection("AI").Get<DomainHttpClientConfiguration>();
            if (config is null)
            {
                throw new Exception("KnownDomains:AI http client configuration is missing.");
            }

            services.AddHttpContextAccessor();
            services.AddTransient<AuthHeaderHandler>();

            services.AddHttpClient<ILearnAIClient, LearnAIClient>(client =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var bearerToken = httpContextAccessor?.HttpContext?.Request
                                      .Headers["Authorization"].FirstOrDefault(h => h != null  
                                                                            && h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase)) ?? string.Empty;

                var language = httpContextAccessor?.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault() ?? "pt-PT";

                client.BaseAddress = new Uri(config.Url);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authentication", bearerToken);
                client.DefaultRequestHeaders.Add("Accept-Language", language);

            }).AddHttpMessageHandler<AuthHeaderHandler>();

            return services;
        }

    }
}

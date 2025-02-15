using Learn.AI.Client.Abstractions;
using Learn.Core.Shared.Clients;
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
                client.BaseAddress = new Uri(config.Url);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddHttpMessageHandler<AuthHeaderHandler>();

            return services;
        }

    }
}

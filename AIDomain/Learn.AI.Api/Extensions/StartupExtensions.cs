using Learn.AI.Repository.MongoDb;
using Learn.AI.Repository.Repositories;
using Learn.AI.Services.Providers.Gpt;
using Learn.AI.Services.Providers.Gpt.Configuration;
using Learn.AI.Services.Providers.Interfaces;
using Learn.Core.Shared.Repository.Configurations;

namespace Learn.AI.Api.Extensions
{
    public static class StartupExtensions
    {
        #region public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<RepositoryConfiguration>(
                    builder.Configuration.GetSection("MongoDbSettings")
            );
            builder.Services.AddTransient<IAIRepository, AIMongoDbRepository>();

            return builder;
        }
        #endregion

        #region public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<OpenAIConfiguration>(
                builder.Configuration.GetSection("OpenAIConfiguration")
            );

            builder.Services.AddTransient<IAIProvider, OpenAIServiceProvider>();
            return builder;
        }
        #endregion

    }
}

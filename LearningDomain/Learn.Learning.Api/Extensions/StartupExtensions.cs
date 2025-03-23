using Learn.Core.Shared.Http;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Learning.Repository.MongoDb.Repository;
using Learn.Learning.Repository.Repositories;
using Learn.Learning.Services;
using Learn.Learning.Services.Interfaces;

namespace Learn.Learning.Api.Extensions
{
    public static class StartupExtensions
    {
        #region public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<RepositoryConfiguration>(
                    builder.Configuration.GetSection("MongoDbSettings")
            );
            builder.Services.AddTransient<IQuestionsRepository, QuestionsMongoDbRepository>();

            return builder;
        }
        #endregion

        #region public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<ILearningService, LearningService>();
            builder.Services.ConfigureCors();

            return builder;
        }
        #endregion

    }
}

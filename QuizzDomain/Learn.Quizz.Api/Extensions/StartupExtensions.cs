using Learn.Core.Shared.Repository.Configurations;
using Learn.Quizz.Repository.MongoDb.Repository;
using Learn.Quizz.Repository.Repositories;
using Learn.Quizz.Services;
using Learn.Quizz.Services.Interfaces;

namespace Learn.Quizz.Api.Extensions
{
    public static class StartupExtensions
    {
        #region public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<RepositoryConfiguration>(
                    builder.Configuration.GetSection("MongoDbSettings")
            );
            builder.Services.AddTransient<IQuizRepository, QuizMongoDbRepository>();

            return builder;
        }
        #endregion

        #region public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<IQuizService, QuizService>();

            builder.Services.AddSingleton<IQuizPublisherService, QuizPublisherService>();
            builder.Services.AddSingleton<IQuizConsumerService, QuizConsumerService>();

            return builder;
        }
        #endregion
    }
}

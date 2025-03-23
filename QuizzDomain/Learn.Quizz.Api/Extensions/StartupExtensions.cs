using Learn.Core.Shared.Http;
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
            builder.Services.AddScoped<IQuizEngine, QuizEngine>();
            builder.Services.ConfigureCors();

            return builder;
        }
        #endregion

        #region public static async Task<WebApplication> EnsureDatabases(this WebApplication app)
        public static async Task<WebApplication> EnsureDatabases(this WebApplication app)
        {
            var quizRepository = app.Services.GetRequiredService<IQuizRepository>();
            await quizRepository.EnsureCollection();

            return app;
        }
        #endregion

    }
}

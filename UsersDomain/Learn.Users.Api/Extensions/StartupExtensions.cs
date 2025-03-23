using Learn.Core.Shared.Http;
using Learn.Core.Shared.Repository.Configurations;
using Learn.Users.Repository.MongoDb;
using Learn.Users.Repository.Repositories;
using Learn.Users.Services;
using Learn.Users.Services.Abstractions.Interfaces;

namespace Learn.Users.Api.Extensions
{
    public static class StartupExtensions
    {
        #region public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddRepositories(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<RepositoryConfiguration>(
                    builder.Configuration.GetSection("MongoDbSettings")
            );
            builder.Services.AddTransient<IUserRepository, UserRepository>();

            return builder;
        }
        #endregion

        #region public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.ConfigureCors();

            return builder;
        }
        #endregion

    }
}

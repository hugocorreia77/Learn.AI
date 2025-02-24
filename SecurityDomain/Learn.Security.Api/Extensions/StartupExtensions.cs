using Learn.Core.Shared.Repository.Configurations;
using Learn.Core.Shared.Security;
using Learn.Core.Shared.Security.Abstractions;
using Learn.Security.Services;
using Learn.Security.Services.Abstractions.Interfaces;
using Learn.Users.Repository.MongoDb;
using Learn.Users.Repository.Repositories;

namespace Learn.Security.Api.Extensions
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
            builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
            builder.Services.AddTransient<IAuthTokenService, AuthTokenService>();
            builder.Services.AddTransient<ISecurityService, SecurityService>();
            return builder;
        }
        #endregion
    }
}

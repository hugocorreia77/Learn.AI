using Learn.Core.Shared.Security.Abstractions;
using Learn.Core.Shared.Services;
using Learn.Core.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Learn.Core.Shared.Security.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            return services;
        }

        public static IHostApplicationBuilder AddLearningAuthentication(this IHostApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            string secret = jwtSettings?["Secret"] ?? string.Empty;
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("JWT Secret settings not found.");
            }
            string issuer = jwtSettings?["Issuer"] ?? string.Empty;
            if (string.IsNullOrEmpty(issuer))
            {
                throw new Exception("JWT Issuer settings not found.");
            }
            string audience = jwtSettings?["Audience"] ?? string.Empty;
            if (string.IsNullOrEmpty(issuer))
            {
                throw new Exception("JWT Audience settings not found.");
            }


            var key = Encoding.UTF8.GetBytes(secret);
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            return builder;
        }

    }
}

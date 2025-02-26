using Learn.Core.Shared.Http;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Core.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Learn.Core.Api.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .ConfigureHttpJsonOptions(options =>
                {
                    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                ;

            return services;
        }

        public static IServiceCollection ConfigureConventions(this IServiceCollection services)
        {
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("EnumStringConvention", pack, t => true);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            return services;
        }

        public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                c.OperationFilter<AddAcceptLanguageHeaderOperationFilter>();
            });

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
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
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
                    options.MapInboundClaims = false;
                });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            return builder;
        }
    }
}

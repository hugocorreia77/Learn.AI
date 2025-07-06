using Learn.Core.Api.Jwt;
using Learn.Core.Shared.Http.Swagger;
using Learn.Core.Shared.Models.Configurations;
using Learn.Core.Shared.Services;
using Learn.Core.Shared.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;

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

        public static IHostApplicationBuilder ConfigureRedis(this IHostApplicationBuilder builder)
        {
            RedisConfiguration redisConfig = GetRedisConfiguration(builder);
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConfig.Address
                ));
            return builder;
        }

        public static IHostApplicationBuilder ConfigureLearningAuthentication(this IHostApplicationBuilder builder)
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
                    //auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

                    //For SignalR authentication
                    // https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-7.0#built-in-jwt-authentication
                    options.Events = new LearnJwtBearerEvents();
                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        var accessToken = context.Request.Query["access_token"];

                    //        // If the request is for our hub...
                    //        var path = context.HttpContext.Request.Path;
                    //        if (!string.IsNullOrEmpty(accessToken) &&
                    //            (path.StartsWithSegments("/quizHub")))
                    //        {
                    //            // Read the token out of the query string
                    //            context.Token = accessToken;
                    //        }
                    //        else
                    //        {
                    //            // falta ir buscar o JWT e atribuir ao context.Token
                    //            // sem isso nao é possivel dar override à autorização


                    //            // Give application opportunity to find from a different location, adjust, or reject token
                    //            //var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);

                    //            //// event can set the token
                    //            //Events.MessageReceived(messageReceivedContext);
                    //            //if (messageReceivedContext.Result != null)
                    //            //{
                    //            //    return messageReceivedContext.Result;
                    //            //}
                    //        }
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            return builder;
        }

        public static IHostApplicationBuilder ConfigureSignalR(this IHostApplicationBuilder builder, bool useRedis = false)
        {

            if (useRedis)
            {
                RedisConfiguration redisConfig = GetRedisConfiguration(builder);
                builder.Services.AddSignalR(
                    options =>
                    {
                        options.EnableDetailedErrors = true;
                        options.MaximumReceiveMessageSize = 102400000;
                    })
                    //.AddMessagePackProtocol()
                    .AddStackExchangeRedis(redisConfig.Address, options => {
                        options.Configuration.ChannelPrefix = new RedisChannel(redisConfig.ChannelPrefix, RedisChannel.PatternMode.Literal);
                    });
            }
            else
            {
                builder.Services.AddSignalR();
            }

            return builder;
        }
    
        private static RedisConfiguration GetRedisConfiguration(IHostApplicationBuilder builder)
        {
            RedisConfiguration? config = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
            if (config is null)
            {
                throw new Exception("Redis configuration not found.");
            }

            return config;
        }

    }
}

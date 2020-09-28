using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Cache.Redis;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.Cache.Redis.Wrapper;

namespace ETMS.WebApi.Extensions
{
    /// <summary>
    /// ServiceCollection扩展类
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加JWT授权
        /// </summary>
        /// <param name="services"></param>
        internal static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SystemConfig.AuthenticationConfig.DefaultAuthenticateScheme;
                options.DefaultChallengeScheme = SystemConfig.AuthenticationConfig.DefaultChallengeScheme;
            }).AddJwtBearer(SystemConfig.AuthenticationConfig.DefaultAuthenticateScheme, jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemConfig.AuthenticationConfig.IssuerSigningKey)),
                    ValidateIssuer = true,
                    ValidIssuer = SystemConfig.AuthenticationConfig.ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = SystemConfig.AuthenticationConfig.ValidAudience,
                    ValidateLifetime = SystemConfig.AuthenticationConfig.ValidateLifetime,
                    ClockSkew = TimeSpan.FromMinutes(SystemConfig.AuthenticationConfig.ClockSkew)
                };
            });
        }

        /// <summary>
        /// 添加Redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConfig"></param>
        internal static void AddRedis(this IServiceCollection services, RedisConfig redisConfig)
        {
            CSRedisWrapper.Initialize(redisConfig.ServerConStrFormat, redisConfig.DbCount);
            services.AddScoped<ICacheProvider, RedisProvider>();
        }
    }
}

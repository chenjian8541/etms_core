using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.Config;
using ETMS.ICache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.Manage.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
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

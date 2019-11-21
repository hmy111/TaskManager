using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTips
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection service,IConfiguration configuration)
        {
            if (configuration["Redis:Enable"].ToLower()=="true")
            {
                var csredis = new CSRedis.CSRedisClient(configuration["Redis:Singleton"]);
                RedisHelper.Initialization(csredis);
            }
            return service;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TaskTips
{
    public static class HttpClientServiceExtensions
    {
        public static IServiceCollection AddHttpClientService(this IServiceCollection service,IConfiguration configuration)
        {
             
            service.AddScoped<HttpClientHandlerMiddleware>();
            service.AddHttpClient();
            //service.AddHttpClient<HttpClientService>()
            //    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(60000), (message, time, retryCount, context) =>
            //{
            //    var msg = $"重试次数： {retryCount}， " +
            //       $" {context.PolicyKey}， " +
            //       $" {context.Keys}， " +
            //       $"原因： {message}。";
            //}))
            //   .AddHttpMessageHandler<HttpClientHandlerMiddleware>()
            //   ;
            //重试
            var retryPolicy = HttpPolicyExtensions
                            .HandleTransientHttpError()
                            .Or<TimeoutRejectedException>() // 若超时则抛出此异常
                            .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(Convert.ToInt32(configuration["HttpSetting:RetryTime"]??"60")*1000), (message, time, retryCount, context) => {
                                var msg = $"重试次数： {retryCount}， " +
                                           $" {context.PolicyKey}， " +
                                           $"原因： {message}。";
                                //logger.LogWarning(msg);
                            });
            //超时
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(Convert.ToInt32(configuration["HttpSetting:TimeOut"] ?? "10"));
            service.AddHttpClient<HttpClientService>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy)
            .AddHttpMessageHandler<HttpClientHandlerMiddleware>();

            return service;
        }
    }
}

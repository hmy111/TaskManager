using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTips
{
    /// <summary>
    /// http 中间件
    /// </summary>
    public class HttpClientHandlerMiddleware : DelegatingHandler
    {
        
        //private IHttpContextAccessor httpContextAccessor;
        private readonly ILogger _logger;
        public HttpClientHandlerMiddleware(ILogger<HttpClientHandlerMiddleware> logger = null)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result= await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation($"请求接口{request.RequestUri}：耗时{stopwatch.Elapsed.TotalMilliseconds}毫秒");
            return result;
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace TaskTips
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;

        public QuartzHostedService(ILogger<QuartzHostedService> logger, IScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"服务启动...{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"服务关闭...{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
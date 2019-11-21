using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Spi;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;

namespace TaskTips
{
    public static class QuartzHostedServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartzHostedService(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddOptions();

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(provider =>
            {
                var option = new QuartzOption(config);
                var sf = new StdSchedulerFactory(option.ToProperties());
                var scheduler = sf.GetScheduler().Result;

                var msf = new StdSchedulerFactory();
                NameValueCollection props = new NameValueCollection();
                props.Add("org.quartz.scheduler.instanceName","");
                msf.Initialize(props);
                var mscheduler = msf.GetScheduler();


                scheduler.JobFactory = provider.GetService<IJobFactory>();
                return scheduler;
            });

            services.AddHostedService<QuartzHostedService>();

            return services;
        }
    }
}
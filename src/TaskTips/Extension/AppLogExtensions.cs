using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTips.Extension
{
    public static class AppLogExtensions
    {
        public static void InitLog(IConfigurationRoot configuration)
        {

            string applicationName = configuration["JobConfig:ServiceName"];
            LoggerConfiguration logconfig = new LoggerConfiguration();
            string ASPNETCORE_ENVIRONMENT = configuration["ASPNETCORE_ENVIRONMENT"];
            if (ASPNETCORE_ENVIRONMENT== "Development")
                logconfig.MinimumLevel.Debug();//最小的输出单位是Debug级别的
            else
                logconfig.MinimumLevel.Information();

            logconfig.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 1024 * 1024 * 200);
            if (configuration["ElasticSearch:Url"] !=null && configuration["ElasticSearch:Enable"]!= "False")
            {

                logconfig.WriteTo.Elasticsearch(
                   new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                   {
                       AutoRegisterTemplate = true,
                       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                       IndexFormat = configuration["ElasticSearch:LogFile"]==null?("etms-" + (ASPNETCORE_ENVIRONMENT == null ? "sit" : ASPNETCORE_ENVIRONMENT.ToLower()) + "-log-{0:yyyy.MM}"): configuration["ElasticSearch:LogFile"].ToLower()
                   });
            }
            
        
           Log.Logger= logconfig
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)//将Microsoft前缀的日志的最小输出级别改成Information
             .Enrich.WithProperty("Application", applicationName)
             .Enrich.FromLogContext()
             .CreateLogger();









        }
    }
}

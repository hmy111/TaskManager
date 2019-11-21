using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace TaskTips
{
    public static class Startup
    {

        public static  Task Run(Action<HostBuilderContext, IServiceCollection> func=null, Action<HostBuilderContext, IConfigurationBuilder> config=null)
        {
             
            InitApplication(() => DefaultBuild(func, config).Run());
            return Task.CompletedTask;
        }
     
        public static void InitApplication(Action action)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile($"appsettings.json", true, true)
               .Build();
           
            var ApplicationName = configuration["JobConfig:ServiceName"];
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.WithProperty("Application", ApplicationName)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = "etms-service" + (configuration["ASPNETCORE_ENVIRONMENT"] == null ? "development" : configuration["ASPNETCORE_ENVIRONMENT"].ToLower()) + "-log-{0:yyyy.MM.dd}"
                    })
                .CreateLogger();

            try
            {
                Log.Information($"Starting {ApplicationName}");
                action.Invoke();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{ApplicationName} terminated unexpectedly!");

            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHost DefaultBuild(Action<HostBuilderContext, IServiceCollection> func = null, Action<HostBuilderContext, IConfigurationBuilder> config = null)
        {
            var host = new HostBuilder()
          .ConfigureHostConfiguration(configHost =>
          {
              configHost.SetBasePath(Directory.GetCurrentDirectory());
          })
          .ConfigureAppConfiguration((hostContext, configApp) =>
          {
              configApp.AddJsonFile("appsettings.json", true,true);
              configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true,true);
              configApp.AddXmlFileForSql();
              if (config != null)
              {
                  config.Invoke(hostContext, configApp);
              }
          })
          .ConfigureServices((hostContext, services) =>
          {
            
              services.AddJob(hostContext.Configuration);//Job服务注入及Job配置
              services.AddHttpClientService(hostContext.Configuration);//httpclient注入
              services.AddSqlMap(hostContext.Configuration);

              services.AddLogging(loggerbuilder => loggerbuilder.AddSerilog(dispose: true));

              services.AddQuartzHostedService(hostContext.Configuration);
              services.AddRedis(hostContext.Configuration);
              if (func != null)
              {
                  func.Invoke(hostContext, services);
              }

          })
          .ConfigureLogging((hostContext, configLogging) =>
          {
              configLogging.AddConsole();
              if (hostContext.HostingEnvironment.EnvironmentName == EnvironmentName.Development)
              {
                  configLogging.AddDebug();
              }
          })
        
          .UseConsoleLifetime();
            return host.Build();
        }
    }
}

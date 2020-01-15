using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using TaskTips.Extension;

namespace TaskTips
{
    public static class Startup
    {
    
        public static void Run(Action<HostBuilderContext, IServiceCollection> func = null, Action<HostBuilderContext, IConfigurationBuilder> config = null)
        {
            InitApplication(() => DefaultBuild(func, config).Run());
        }
        public static void InitApplication(Action action)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile($"appsettings.json", true, true)
               .Build();

            AppLogExtensions.InitLog(configuration);
            try
            {
                Log.Information($"Starting App");
                action.Invoke();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"App terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHost DefaultBuild(Action<HostBuilderContext, IServiceCollection> func = null, Action<HostBuilderContext, IConfigurationBuilder> config = null)
        {
            var host = new HostBuilder()
          .UseServiceProviderFactory(new AutofacServiceProviderFactory())
          .ConfigureHostConfiguration(configHost =>
          {
              configHost.SetBasePath(Directory.GetCurrentDirectory());
          })
          .ConfigureAppConfiguration((hostContext, configApp) =>
          {
              configApp.AddJsonFile("appsettings.json", true, true);
              configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true);
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
          .ConfigureContainer<ContainerBuilder>(_builder =>
          {
              _builder.RegisterModule<ApplicationModule>();
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

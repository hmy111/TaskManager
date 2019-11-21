using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TaskTips.Common;

namespace TaskTips
{
    public static class JobExtensions
    {
        public static IServiceCollection AddJob(this IServiceCollection service, IConfiguration _configuration)
        {
            
            DirectoryInfo Dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] fileInfo = Dir.GetFiles(GetPattern(), SearchOption.TopDirectoryOnly);

            foreach (FileInfo item in fileInfo)
            {
                Type[] alltypes = Assembly.LoadFrom(item.FullName).GetTypes();
                var types = alltypes.Where(t => t.IsSubclassOf(typeof(JobSerivce))).ToArray();
                foreach (var typechild in types)
                {
                    service.AddSingleton(typechild);
                }
                service.AddConfig(alltypes, _configuration);//自动注入配置参数
            }
            return service;
        }

        private static string GetPattern()
        {
            var domainName = AppDomain.CurrentDomain.FriendlyName;
            var index = domainName.IndexOf(".");
            if (index == -1)
            {
                return domainName;
            }
            var result = domainName.Substring(0, index);
            return $"{result}*.dll";
        }

        private static IServiceCollection AddConfig(this IServiceCollection service,Type[] types, IConfiguration _configuration)
        {
            var typeConfigs= types.Where(m => m.IsSubclassOf(typeof(JobConfig))).ToArray();
            foreach (var item in typeConfigs)
            {
                service.AddScoped(item,(p)=> {
                    var activatorItem = Activator.CreateInstance(item);
                    _configuration.GetSection("JobConfig").Bind(activatorItem);
                    return activatorItem;
                });
            }
            return service;
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using TaskTips;
namespace Task.Manager.Demo
{
    public class DemoJob : TaskTips.JobSerivce
    {
        public readonly HttpClientService _httpClientService;//http请求
        public readonly AppSettingConfig _parameter;//任务参数
        public readonly SqlConfig _sqlConfig;//获取sql
        public DemoJob(IConfiguration configuration, SqlConfig sqlConfig, HttpClientService httpClientService) : base(configuration)
        {
            _sqlConfig = sqlConfig;
            _httpClientService = httpClientService;
        }

        public override System.Threading.Tasks.Task ExecuteJob(IJobExecutionContext context)
        {
            var ss = _sqlConfig.GetSql("GetVehicles");
            _logger.LogInformation(ss);//写日志
            var result= _httpClientService.HttpGetJson<String>("http://www.baid.com");//发送请求

            //do something ..
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}

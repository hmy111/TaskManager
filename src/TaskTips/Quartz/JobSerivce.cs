using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskTips
{
    public abstract class JobSerivce : IJob
    {
        public readonly IConfiguration _configuration;
        public readonly SqlMappings _mappings;//获取mappings 数据库脚本配置
        protected ILogger _logger;
        private static readonly object obj = new object();

        protected JobSerivce(IConfiguration configuration, ILogger<JobSerivce> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        #region 废弃方法

        /// <summary>
        /// 获取xml脚本 根据map里的name
        /// </summary>
        /// <param name="xmlname"></param>
        [Obsolete]
        public virtual string GetSqlByID(string Id)
        {
            return AnalysisXml(Id);
        }
        /// <summary>
        /// 获取xml脚本 根据map里的name //
        /// </summary>
        /// <param name="xmlname"></param>
        [Obsolete]
        public virtual string GetSql(string Id)
        {
            return AnalysisXml(Id);
        }
        /// <summary>
        /// 解析xml （待扩展）
        /// </summary>
        /// <param name="xmlName"></param>
        /// <returns></returns>
        [Obsolete]
        private string AnalysisXml(string xmlName)
        {
            return _configuration[$"map:{xmlName}:sql"];
        }
        #endregion
        public Task Execute(IJobExecutionContext context)
        {

            lock (obj)
            {
                try
                {
                    _logger.LogInformation($"开始本次轮询：时间{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    ExecuteJob(context);
                    stopwatch.Stop();
                    _logger.LogInformation($"结束本次轮询：时间{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},本轮时效监控服务超时，耗时{stopwatch.Elapsed.TotalMilliseconds}毫秒");
                    //Console.WriteLine("Using Elapsed output runTime:{0}", stopwatch.Elapsed.ToString());//这里使用时间差来输出
                }
                catch (Exception ex)
                {
                    //记录日志全局日志
                    _logger.LogError(ex.ToString());
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public abstract Task ExecuteJob(IJobExecutionContext context);
    }


}

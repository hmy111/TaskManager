using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TaskTips
{
    /// <summary>
    /// More details:
    /// https://github.com/quartznet/quartznet/blob/master/src/Quartz/Impl/StdSchedulerFactory.cs
    /// </summary>
    public class QuartzOption
    {
       
        public QuartzOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection("quartz");
            
            section.Bind(this);
        }

        public Scheduler Scheduler { get; set; }

        public ThreadPool ThreadPool { get; set; }

        public Plugin Plugin { get; set; }

        public NameValueCollection ToProperties()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = Scheduler?.InstanceName,
                ["quartz.threadPool.type"] = ThreadPool?.Type?? "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadPriority"] = ThreadPool?.ThreadPriority?? "Normal",
                ["quartz.threadPool.threadCount"] = ThreadPool?.ThreadCount.ToString()??"10",
                ["quartz.plugin.jobInitializer.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
                ["quartz.plugin.jobInitializer.fileNames"] = "quartz_jobs.xml"
            };

            return properties;
        }
    }
  //   "quartz": {
  //  "scheduler": {
  //    "instanceName": "YH.TaskManager.GPSVehicleLocation"
  //  },
  //  "threadPool": {
  //    "type": "Quartz.Simpl.SimpleThreadPool, Quartz",
  //    "threadPriority": "Normal",
  //    "threadCount": 10
  //  },
  //  "plugin": {
  //    "jobInitializer": {
  //      "type": "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
  //      "fileNames": "quartz_jobs.xml"
  //    }
  //  }
  //},
    public class Scheduler
    {
        public string InstanceName { get; set; }
    }

    public class ThreadPool
    {
        public string Type { get; set; }

        public string ThreadPriority { get; set; }

        public int ThreadCount { get; set; }
    }

    public class Plugin
    {
        public JobInitializer JobInitializer { get; set; }
    }

    public class JobInitializer
    {
        public string Type { get; set; }
        public string FileNames { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using TaskTips.Common;

namespace Task.Manager.Demo
{
    //继承JobConfig 自动会把JobConfig 配置注入
    public class AppSettingConfig: JobConfig
    {
        public string ServiceName { get; set; }
    }
}

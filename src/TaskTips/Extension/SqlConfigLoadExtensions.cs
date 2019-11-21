using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace TaskTips
{
    /// <summary>
    /// 数据库查询脚本
    /// </summary>
    public static class SqlConfigLoadExtensions
    {
        public static IConfigurationBuilder AddXmlFileForSql(this IConfigurationBuilder configuration)
        {
            DirectoryInfo Dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] fileInfo = Dir.GetFiles("Common_*.xml", SearchOption.AllDirectories);
            foreach (FileInfo item in fileInfo)
            {
                configuration.AddXmlFile(item.FullName,true,true);
            }
            return configuration;
        }
    }
}

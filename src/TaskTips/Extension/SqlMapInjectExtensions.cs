using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TaskTips
{
    public static class SqlMapInjectExtensions
    {
        public static IServiceCollection AddSqlMap(this IServiceCollection service,IConfiguration configuration)
        {
            DirectoryInfo Dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] fileInfo = Dir.GetFiles("Common_*.xml", SearchOption.AllDirectories);
            SqlConfig dtos = new SqlConfig();
         
            foreach (FileInfo item in fileInfo)
            {
                var strXml = File.ReadAllText(item.FullName);
                SqlMappings dto = DESerializerStringToEntity<SqlMappings>(strXml);
                dto.SetFileUrl(item.FullName);
                AddSqlMap(dtos.SqlMappings, dto);
            }

            service.AddSingleton(dtos);
            return service;
        }
        //转换检验脚本
        private static void AddSqlMap(List<SqlMappings> list, SqlMappings map)
        {

            var dto = list.Find(m => m.NameSpace == map.NameSpace && m.DbType == map.DbType);
            if (dto==null)
            {
                list.Add(map);
            }
            else
            {
                foreach (var item in map?.Maps)
                {
                    dto.Maps= dto?.Maps ?? new List<SqlMap>();
                    if(dto.Maps.Exists(e => e.Name == item.Name))
                    {
                        dto.Maps.Add(item);
                    }
                    else
                    {
                        throw new Exception($"存在重复的sql name 配置 url:{item.FileUrl},name:{item.Name}");
                    }
                }
            }
            
        } 
        public static T DESerializerStringToEntity<T>(string strXML) where T : class
        {
            using (StringReader sr = new StringReader(strXML))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(sr) as T;
            }

        }

        
    }
}

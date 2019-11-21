using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TaskTips.Common;
namespace TaskTips
{
    public enum Enum_SqlType
    {
        [XmlEnum(Name = "sqlserver")]
        SqlServer = 1,
        [XmlEnum(Name = "oracle")]
        Oracle = 2,
        [XmlEnum(Name = "mysql")]
        Mysql = 3
    }
    public class SqlConfig
    {
        public SqlConfig()
        {
            this.SqlMappings = new List<SqlMappings>();
        }

        public List<SqlMappings> SqlMappings { get; set; }

        public string GetSql(string MapName,string NameSpace="",Enum_SqlType? type=null)
        {
            if (string.IsNullOrWhiteSpace(MapName))
            {
                return "";
            }
            var result = this.SqlMappings;
            if (!string.IsNullOrWhiteSpace(NameSpace))
            {
                result =  result.Where(f => f.NameSpace == NameSpace).ToList();
            }
            if (type!=null && type!=0)
            {
                result = result.Where(f => f.DbType == type).ToList();
            }
          
            var map = result.SelectMany(p => p.Maps).FirstOrDefault(f=>f.Name.ToLower()==MapName.ToLower());
            
            return map == null?"": map.Sql;
        }
    }

    [Serializable]
    [XmlRoot("configuration")]
    public class SqlMappings : ISqlMappings
    {
        /// <summary>
        /// 脚本集合Dto
        /// </summary>
        [XmlArray("mappings"), XmlArrayItem("map")]
        public List<SqlMap> Maps { get; set; }
        [XmlAttribute("namepsaces")]
        public string NameSpace { get; set; }
        [XmlAttribute("dbtype")]
        public Enum_SqlType DbType { get; set; }
        /// <summary>
        /// 设置发现文件路径
        /// </summary>
        /// <param name="FileUrl"></param>
        /// <returns></returns>
        public bool SetFileUrl(string FileUrl)
        {
            this.Maps.ForEach(i => i.FileUrl = FileUrl);
            if (this.DbType == 0)
            {
                foreach (string item in Enum.GetNames(typeof(Enum_SqlType)))
                {
                    if (FileUrl.ToLower().IndexOf($@"\{item.ToLower()}\") >= 0)
                    {
                        this.DbType = Enum.Parse<Enum_SqlType>(item);
                        break;
                    }
                }
            }
            return true;
        }
    }
    [Serializable]
    [XmlRoot(ElementName = "map")]
    public class SqlMap
    {
        [XmlArray("schema"), XmlArrayItem("option")]
        public List<SchemaOption> Schema { get; set; }
        [XmlElement("sql")]
        public string Sql { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("auth")]
        public string Author { get; set; }
        /// <summary>
        /// 同一个命名空间下必须唯一
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        public string FileUrl { get; set; }
    }
    [XmlRoot(ElementName = "option")]
    public class SchemaOption
    {
        /// <summary>
        /// 必须唯一 在同个项目下
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}

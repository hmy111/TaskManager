using System;
using System.Collections.Generic;
using System.Text;

namespace TaskTips
{
    public static class NullableExtension
    {
        public static int ToInt(this int? obj)
        {
            return Convert.ToInt32(obj);
        }
        public static DateTime ToDateTime(this DateTime? obj)
        {
            return Convert.ToDateTime(obj);
        }
        public static Boolean ToBoolean(this Boolean? obj)
        {
            return Convert.ToBoolean(obj);
        }
        public static string GetEnumName(this Enum obj)
        {
            return obj.GetType().GetEnumName(obj);
        }
        public static int GetEnumValue(this Enum obj)
        {
            return Convert.ToInt32(obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace ETMS.Pay.Suixing.Utility
{
    public class PackReflectionEntity<T>
    {
        /// <summary>
        /// 将实体类通过反射组装成字符串
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns>组装的字符串</returns>
        public static string GetEntityToString(T t)
        {
            var sb = new StringBuilder();
            var propertyInfos = t.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var value = propertyInfos[i].GetValue(t, null);
                if (propertyInfos[i].PropertyType.Name.StartsWith("String"))//判斷字段類型
                {
                    if (value != null)
                    {
                        var result = value.ToString();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            sb.Append($"{propertyInfos[i].Name }={result}&");
                        }
                    }
                }
                else
                {
                    string result = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    sb.Append($"{propertyInfos[i].Name}={result}&");
                }

            }
            return sb.ToString().TrimEnd(new char[] { '&' });
        }
    }
}

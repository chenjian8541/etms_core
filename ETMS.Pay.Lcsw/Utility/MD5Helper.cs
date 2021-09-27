using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ETMS.Pay.Lcsw.Utility
{
    public class MD5Helper
    {

        /// <summary>
        /// 获取签名字符串
        /// </summary>
        /// <param name="text">需要签名的字符串</param>
        /// <returns>小写)签名结果</returns>
        public static string GetSign(string text)
        {
            string signStr = string.Empty;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bResult = md5.ComputeHash(getContentBytes(text));
            signStr = BitConverter.ToString(bResult).Replace("-", "");
            return signStr.ToLower();
        }

        /// <summary>
        /// 获取签名字符串
        /// </summary>
        /// <param name="text">需要签名的字符串</param>
        /// <param name="key">令牌</param>
        /// <returns>(小写)签名结果</returns>
        public static string GetSign(string text, string key)
        {
            string signStr = string.Empty;
            text = text + "&access_token=" + key;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bResult = md5.ComputeHash(getContentBytes(text));
            signStr = BitConverter.ToString(bResult).Replace("-", "");
            return signStr.ToLower();
        }

        public static string GetSign(Dictionary<string, object> dic, string tokenKey, string tokenValue)
        {
            StringBuilder sb = new StringBuilder();
            dic = dic.OrderBy(a => a.Key).ToDictionary(o => o.Key, p => p.Value);
            foreach (var item in dic)
            {
                if (item.Value != null)
                {
                    sb.Append(item.Key + "=" + item.Value + "&");
                }
            }
            sb.AppendFormat("{0}={1}", tokenKey, tokenValue);
            var md5 = System.Security.Cryptography.MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            var str = new StringBuilder();
            foreach (var b in bs)
            {
                str.Append(b.ToString("x2"));
            }
            return str.ToString().ToUpper();
        }

        ///// <summary>
        ///// 获取签名字符串
        ///// 参数以字典排序
        ///// </summary>
        ///// <param name="paramsMap"></param>
        ///// <param name="tokenKey"></param>
        ///// <param name="tokenValue"></param>
        ///// <returns></returns>
        //public static string GetSign(Dictionary<string, object> paramsMap, string tokenKey, string tokenValue)
        //{
        //    if (string.IsNullOrEmpty(tokenKey))
        //    {
        //        throw new Exception("tokenKey不能为空");
        //    }
        //    if (string.IsNullOrEmpty(tokenValue))
        //    {
        //        throw new Exception("tokenValue不能为空");
        //    }
        //    var  paramsMaps= (from objDic in paramsMap orderby objDic.Key ascending select objDic);
        //    StringBuilder str = new StringBuilder();
        //    foreach (var kv in paramsMaps)
        //    {
        //        string pkey = kv.Key;
        //        string pvalue = kv.Value.ToString();
        //        if (string.IsNullOrEmpty(pkey) || string.IsNullOrEmpty(pvalue))
        //        {
        //            continue;
        //        }
        //        str.AppendFormat("{0}={1}&", pkey, pvalue);
        //    }
        //    str.AppendFormat("{0}={1}",tokenKey,tokenValue);
        //    return GetSign(str.ToString());
        //}

        /// <summary>
        /// 获取签名字符串
        /// 没有排序
        /// </summary>
        /// <param name="paramsMap">参数</param>
        /// <param name="tokenKey">令牌Key</param>
        /// <param name="tokenValue">令牌Value</param>
        /// <returns></returns>
        public static string GetSignNoSort(Dictionary<string, object> paramsMap, string tokenKey, string tokenValue)
        {
            StringBuilder str = new StringBuilder();
            foreach (var kv in paramsMap)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value.ToString();
                if (string.IsNullOrEmpty(pkey) || string.IsNullOrEmpty(pvalue))
                {
                    continue;
                }
                str.AppendFormat("{0}={1}&", pkey, pvalue);
            }
            str.AppendFormat("{0}={1}", tokenKey, tokenValue);
            return GetSign(str.ToString());
        }

        /// <summary>
        /// 获取签名字符串
        /// 参数以字典排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="tokenKey">令牌Key</param>
        /// <param name="tokenValue">令牌Value</param>
        /// <returns></returns>
        public static string GetSign<T>(T t, string tokenKey, string tokenValue)
        {
            string signStr = string.Empty;
            var paramsMap = GetEntityToDictionary(t);
            var paramsMaps = (from objDic in paramsMap orderby objDic.Key ascending select objDic);
            StringBuilder str = new StringBuilder();
            foreach (var kv in paramsMaps)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value.ToString();
                str.AppendFormat("{0}={1}&", pkey, pvalue);
            }
            str.AppendFormat("{0}={1}", tokenKey, tokenValue);
            return GetSign(str.ToString());
        }

        /// <summary>
        /// 获取字符串Bytes数组,字符编码统一为UTF-8
        /// </summary>
        /// <param name="strString"></param>
        /// <returns></returns>
        private static byte[] getContentBytes(string strString)
        {
            if (string.IsNullOrEmpty(strString))
            {
                throw new Exception("");
            }
            return Encoding.UTF8.GetBytes(strString);
        }

        /// <summary>
        /// 实体类转换为key-value
        /// 不转换为null的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetEntityToDictionary<T>(T t)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            Type type = t.GetType();
            System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].GetValue(t) != null)
                {
                    map.Add(propertyInfos[i].Name, propertyInfos[i].GetValue(t, null));
                }
            }
            return map;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Com.Fubei.OpenApi.Sdk.Enums;
using Com.Fubei.OpenApi.Sdk.Models;

namespace Com.Fubei.OpenApi.Sdk.Utils
{
    /// <summary>
    /// 开放平台签名工具类
    /// </summary>
    public static class FubeiSignatureUtil
    {
        private static string ToBaseString(Dictionary<string, string> map, string appSecret)
        {
            var sb = new StringBuilder();
            map.OrderBy(p => p.Key).ToList().ForEach(pair => sb.AppendFormat("{0}={1}&", pair.Key, pair.Value));
            sb.Remove(sb.Length - 1, 1).Append(appSecret);
            return sb.ToString();
        }

        private static string ToBaseString(EApiLevel level, FubeiRequestParam requestParam)
        {
            var map = new Dictionary<string, string>
            {
                {"method", requestParam.Method},
                {"format", requestParam.Format},
                {"sign_method", requestParam.SignMethod},
                {"nonce", requestParam.Nonce},
                {"version", requestParam.Version},
                {"biz_content", requestParam.BizContent}
            };

            if (level == EApiLevel.Merchant)
            {
                map.Add("app_id", requestParam.AppId);
            }
            else
            {
                map.Add("vendor_sn", requestParam.VendorSn);
            }

            return ToBaseString(map, requestParam.AppSecret);
        }
        
        private static string ToBaseString(FubeiApiCommonResult<string> responseResult, string appSecret)
        {
            var map = new Dictionary<string, string>
            {
                {"result_code", responseResult.ResultCode},
                {"result_message", responseResult.ResultMessage},
                {"data", responseResult.Data}
            };
            return ToBaseString(map, appSecret);
        }

        private static string Hash(string str)
        {
            var bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="apiLevel">接口级别</param>
        /// <param name="requestParam">请求参数</param>
        public static void DoSign(EApiLevel apiLevel, ref FubeiRequestParam requestParam)
        {
            var baseString = ToBaseString(apiLevel, requestParam);
            requestParam.Sign = Hash(baseString);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="result"></param>
        /// <param name="appSecret"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static bool Verify(FubeiApiCommonResult<string> result, string appSecret, string sign)
        {
            var signExpected = Hash(ToBaseString(result, appSecret));
            return signExpected.Equals(sign, StringComparison.OrdinalIgnoreCase);
        }
    }
}
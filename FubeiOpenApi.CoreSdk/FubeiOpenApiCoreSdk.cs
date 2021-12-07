using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Com.Fubei.OpenApi.Sdk.Enums;
using Com.Fubei.OpenApi.Sdk.Models;
using Com.Fubei.OpenApi.Sdk.Utils;
using Com.Fubei.OpenApi.Sdk.Extensions;
using Newtonsoft.Json;
using ETMS.LOG;

namespace Com.Fubei.OpenApi.Sdk
{
    /// <summary>
    /// 付呗OpenApi SDK (.net core 2.2)
    /// </summary>
    public static class FubeiOpenApiCoreSdk
    {
        /// <summary>
        /// 异步POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static async Task<T> PostRequestAsync<T>(string url, FubeiBizParam param)
        {
            var requestParam = param.GenerateAsFubeiRequestParam();
            Log.Debug($"[付呗请求]rul:{url}", param, typeof(FubeiOpenApiCoreSdk));
            var json = await HttpUtil.PostRequest(url, requestParam);
            Log.Debug($"[付呗返回]rul:{url}", json, typeof(FubeiOpenApiCoreSdk));
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 验证付呗通知回调
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sign"></param>
        /// <param name="apiLevel"></param>
        /// <returns></returns>
        public static bool VerifyFubeiNotification(FubeiApiCommonResult<string> result, string sign,
            EApiLevel apiLevel)
        {
            var secret = apiLevel == EApiLevel.Merchant
                ? result.AppSecret
                : result.VendorSecret;

            return FubeiSignatureUtil.Verify(result, secret, sign);
        }

        /// <summary>
        /// 获得Api地址
        /// </summary>
        /// <param name="openApiVersion">接口版本号 1: 开放平台1.0  2: 开放平台2.0</param>
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        private static string GetApiUrlByVersion(int openApiVer = 2)
        {
            return openApiVer == 1 ? FubeiOpenApiGlobalConfig.Instance.Api_1_0 : FubeiOpenApiGlobalConfig.Instance.Api_2_0;
        }

        public static async Task<FubeiApiCommonResult<T>> PostMerchantApiRequestAsync<T>(string method, BaseEntity obj) where T : new()
        {
            return await PostRequestAsync<FubeiApiCommonResult<T>>(GetApiUrlByVersion(1), obj.WithFubeiBizParam(method, EApiLevel.Merchant));
        }

        public static async Task<FubeiApiCommonResult<T>> PostVendorApiRequestAsync<T>(string method, BaseEntity obj, EApiLevel apiLevel = EApiLevel.Vendor) where T : new()
        {
            return await PostRequestAsync<FubeiApiCommonResult<T>>(GetApiUrlByVersion(2), obj.WithFubeiBizParam(method, apiLevel));
        }
    }
}

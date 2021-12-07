using System;
using System.Collections.Generic;
using System.Text;
using Com.Fubei.OpenApi.Sdk.Enums;
using Com.Fubei.OpenApi.Sdk.Models;
using Com.Fubei.OpenApi.Sdk.Utils;

namespace Com.Fubei.OpenApi.Sdk.Extensions
{
    /// <summary>
    /// 付呗参数生成-扩展类
    /// </summary>
    public static class FubeiBizParamExtension
    {
        private static readonly FubeiOpenApiGlobalConfig ApiConfig = FubeiOpenApiGlobalConfig.Instance;
        
        public static FubeiRequestParam GenerateAsFubeiRequestParam<T>(this T bizParam) where T : FubeiBizParam
        {
            var requestParam = new FubeiRequestParam
            {
                BizContent = (bizParam.BizContent ?? BaseEntity.Empty).SerializeAsJson(),
                Nonce = RandomStringUtil.RandomAlphabetAndNumeric(16),
                Method = bizParam.Method
            };
            switch (bizParam.ApiLevel)
            {
                // 商户级
                case EApiLevel.Merchant:
                default:
                    requestParam.AppId = bizParam.AppId;
                    requestParam.AppSecret = bizParam.AppSecret;
                    requestParam.VendorSn = null;
                    break;

                // 服务商级
                case EApiLevel.Vendor:
                    requestParam.VendorSn = bizParam.VendorSn;
                    requestParam.AppSecret = bizParam.VendorSecret;
                    requestParam.AppId = null;
                    break;
            }


            // 对请求参数进行签名
            FubeiSignatureUtil.DoSign(bizParam.ApiLevel, ref requestParam);
            return requestParam;
        }
    }
}

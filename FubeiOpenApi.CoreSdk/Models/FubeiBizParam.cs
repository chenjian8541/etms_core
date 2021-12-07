using Com.Fubei.OpenApi.Sdk.Enums;
using Newtonsoft.Json;

namespace Com.Fubei.OpenApi.Sdk.Models
{
    /// <summary>
    /// 付呗基本业务参数（带openapi method）
    /// </summary>
    public class FubeiBizParam
    {
        internal FubeiBizParam(string method, BaseEntity obj, EApiLevel apiLevel)
        {
            this.Method = method;
            this.BizContent = obj;
            this.ApiLevel = apiLevel;
            this.AppId = obj.AppId;
            this.AppSecret = obj.AppSecret;
            this.VendorSn = obj.VendorSn;
            this.VendorSecret = obj.VendorSecret;
        }

        public EApiLevel ApiLevel { get; }

        public string Method { get; }

        public BaseEntity BizContent { get; }

        /// <summary>
        /// 商户Api AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户Api AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 服务商SN
        /// </summary>
        public string VendorSn { get; set; }

        /// <summary>
        /// 服务商AppSecret
        /// </summary>
        public string VendorSecret { get; set; }
    }

    public static class FubeiBizParamExtension
    {
        public static FubeiBizParam WithFubeiBizParam<T>(this T a, string method, EApiLevel apiLevel) where T : BaseEntity
        {
            return new FubeiBizParam(method, a, apiLevel);
        }
    }
}
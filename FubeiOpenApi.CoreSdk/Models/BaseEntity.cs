using System;

namespace Com.Fubei.OpenApi.Sdk.Models
{
    /// <summary>
    /// 类型限定，方便后续扩展
    /// </summary>
    public interface IBaseModel
    {
    }

    /// <summary>
    /// 类型限定，方便后续扩展
    /// </summary>
    [Serializable]
    public class BaseEntity : IBaseModel
    {
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

        public static readonly BaseEntity Empty = new BaseEntity();
    }
}

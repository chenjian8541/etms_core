using System;

namespace WxApi.ReceiveEntity
{
    /// <summary>
    /// 全局票据实体
    /// </summary>
    public class AccessToken : ErrorEntity
    {
        /// <summary>
        /// AccessToken的值
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }
}

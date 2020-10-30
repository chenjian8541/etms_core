using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 微信开放平台AuthorizerToken
    /// </summary>
    [Table("SysWechartAuthorizerToken")]
    public class SysWechartAuthorizerToken : EManageEntity<int>
    {
        /// <summary>
        /// 授权方 appid
        /// </summary>
        public string AuthorizerAppid { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string AuthorizerRefreshToken { get; set; }

        /// <summary>
        /// 第三方平台appid
        /// </summary>
        public string ComponentAppId { get; set; }

        /// <summary>
        /// 接口调用令牌
        /// </summary>
        public string AuthorizerAccessToken { get; set; }

        /// <summary>
        /// authorizer_access_token 的有效期  单位：秒
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyOt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 机构公众号授权信息
    /// </summary>
    [Serializable]
    [Table("SysTenantWechartAuth")]
    public class SysTenantWechartAuth : EManageEntity<int>
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 授权方appid
        /// </summary>
        public string AuthorizerAppid { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImg { get; set; }

        /// <summary>
        /// 公众号类型  <see cref="ETMS.Entity.Enum.EmWechartAuthServiceTypeInfo"/>
        /// </summary>
        public string ServiceTypeInfo { get; set; }

        /// <summary>
        /// 公众号认证类型  <see cref="ETMS.Entity.Enum.EmWechartAuthVerifyTypeInfo"/>
        /// </summary>
        public string VerifyTypeInfo { get; set; }

        /// <summary>
        /// 原始 ID
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 主体名称
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// 公众号所设置的微信号，可能为空
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 用以了解功能的开通状况
        /// </summary>
        public string BusinessInfo { get; set; }

        /// <summary>
        /// 二维码图片的 URL
        /// </summary>
        public string QrcodeUrl { get; set; }

        /// <summary>
        /// 授权信息
        /// </summary>
        public string PermissionsKey { get; set; }

        /// <summary>
        /// 授权信息
        /// </summary>
        public string PermissionsValue { get; set; }

        /// <summary>
        /// 授权状态 <see cref="ETMS.Entity.Enum.EmSysTenantWechartAuthAuthorizeState"/>
        /// </summary>
        public byte AuthorizeState { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyOt { get; set; }
    }
}

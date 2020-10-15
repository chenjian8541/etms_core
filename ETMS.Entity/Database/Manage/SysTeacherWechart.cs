using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 老师端绑定机构
    /// </summary>
    [Table("SysTeacherWechart")]
    public class SysTeacherWechart: EManageEntity<long>
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 微信Unionid
        /// </summary>
        public string WechatUnionid { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public string WechatOpenid { get; set; }
    }
}

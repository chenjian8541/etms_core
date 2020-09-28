using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 家长微信绑定机构
    /// </summary>
    [Table("SysStudentWechart")]
    public class SysStudentWechart
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 数据状态  <see cref="ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }
    }
}

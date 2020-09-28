using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 家长通知管理
    /// </summary>
    [Table("EtStudentNotice")]
    public class EtStudentNotice : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmStudentNoticeType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 微信Unionid
        /// </summary>
        public string WechatUnionid { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public string WechatOpenid { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string NoticeContent { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}

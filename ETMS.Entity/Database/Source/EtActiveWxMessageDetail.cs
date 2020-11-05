using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveWxMessageDetail")]
    public class EtActiveWxMessageDetail : Entity<long>
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 主表ID
        /// </summary>
        public long WxMessageId { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否需要家长确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        /// <summary>
        /// 是否确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsConfirm { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmOt { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

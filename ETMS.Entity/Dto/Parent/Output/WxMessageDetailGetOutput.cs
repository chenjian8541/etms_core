using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class WxMessageDetailGetOutput
    {
        public long WxMessageDetailId { get; set; }

        /// <summary>
        /// 主表ID
        /// </summary>
        public long WxMessageId { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员名称
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否已读 <see cref="EmBool"/>
        /// </summary>
        public byte IsRead { get; set; }

        /// <summary>
        /// 是否需要家长确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        /// <summary>
        /// 是否确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsConfirm { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MessageContent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveWxMessageGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 接收者类型  <see cref="EmActiveWxMessageType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public string RelatedDesc { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary> 
        /// 是否需要家长确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 已确认数量
        /// </summary>
        public int ConfirmCount { get; set; }

        /// <summary>
        /// 已读数量
        /// </summary>
        public int ReadCount { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveWxMessage")]
    public class EtActiveWxMessage: Entity<long>
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 接收者类型  <see cref="EmActiveWxMessageType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 接收者关联的ID集合
        /// </summary>
        public string RelatedIds { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MessageContent { get; set; }

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

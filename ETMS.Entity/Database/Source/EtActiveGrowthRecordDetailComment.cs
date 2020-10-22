using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveGrowthRecordDetailComment")]
    public class EtActiveGrowthRecordDetailComment : Entity<long>
    {
        /// <summary>
        /// 成长档案ID
        /// </summary>
        public long GrowthRecordId { get; set; }

        /// <summary>
        /// 成长档案详情ID
        /// </summary>
        public long GrowthRecordDetailId { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public long? ReplyId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentContent { get; set; }

        /// <summary>
        /// 数据来源 <see cref="ETMS.Entity.Enum.EmActiveCommentSourceType"/>
        /// </summary>
        public byte SourceType { get; set; }

        /// <summary>
        /// 来源ID
        /// </summary>
        public long SourceId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

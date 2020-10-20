using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 作业评论
    /// </summary>
    [Table("EtActiveHomeworkDetailComment")]
    public class EtActiveHomeworkDetailComment : Entity<long>
    {
        /// <summary>
        /// 作业ID
        /// </summary>
        public long HomeworkId { get; set; }

        /// <summary>
        /// 作业详情ID
        /// </summary>
        public long HomeworkDetailId { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public long? ReplyId { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentContent { get; set; }

        /// <summary>
        ///来源类型  <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailCommentSourceType"/>
        /// </summary>
        public byte SourceType { get; set; }

        /// <summary>
        /// 来源ID
        /// </summary>
        public long SourceId { get; set; }

        /// <summary>
        /// 如果是学生则记录家长的昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 如果是学生则记录家长的头像
        /// </summary>
        public string Headimgurl { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

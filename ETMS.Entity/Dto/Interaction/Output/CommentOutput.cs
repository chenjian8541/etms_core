using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class CommentOutput
    {
        public long CommentId { get; set; }

        public string CommentContent { get; set; }

        public byte SourceType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string RelatedManName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string RelatedManAvatar { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public string Ot { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public string OtDesc { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public long? ReplyId { get; set; }

        public string ReplyRelatedManName { get; set; }
    }
}

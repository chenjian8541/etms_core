using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthCommentAddRequest : RequestBase
    {
        public long GrowthRecordId { get; set; }

        public long? ReplyId { get; set; }

        public string CommentContent { get; set; }

        public override string Validate()
        {
            if (GrowthRecordId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(CommentContent))
            {
                return "请输入评论内容";
            }
            return base.Validate();
        }
    }
}

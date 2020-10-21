using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class HomeworkAddCommentRequest : ParentRequestBase
    {
        public long HomeworkDetailId { get; set; }

        public string CommentContent { get; set; }

        public override string Validate()
        {
            if (HomeworkDetailId <= 0)
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(CommentContent))
            {
                return "请填写评论信息";
            }
            return string.Empty;
        }
    }
}

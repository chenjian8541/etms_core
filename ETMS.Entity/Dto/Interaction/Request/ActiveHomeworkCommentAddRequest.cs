using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkCommentAddRequest : RequestBase
    {
        public long HomeworkId { get; set; }

        public long HomeworkDetailId { get; set; }

        public long? ReplyId { get; set; }

        public string CommentContent { get; set; }

        public override string Validate()
        {
            if (HomeworkId <= 0 || HomeworkDetailId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(CommentContent))
            {
                return "请输入评语内容";
            }
            return base.Validate();
        }
    }
}

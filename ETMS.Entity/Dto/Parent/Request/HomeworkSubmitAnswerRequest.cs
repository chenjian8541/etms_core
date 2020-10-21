using ETMS.Entity.Common;
using System;
using System.Collections.Generic;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class HomeworkSubmitAnswerRequest : ParentRequestBase
    {
        public long HomeworkDetailId { get; set; }

        public string AnswerContent { get; set; }

        public List<string> AnswerMediasKeys { get; set; }

        public override string Validate()
        {
            if (HomeworkDetailId <= 0)
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(AnswerContent) && (AnswerMediasKeys == null || AnswerMediasKeys.Count == 0))
            {
                return "请填写作答信息";
            }
            return string.Empty;
        }
    }
}


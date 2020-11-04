using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class GrowthRecordDelCommentRequest : ParentRequestBase
    {
        public long GrowthRecordDetailId { get; set; }

        public long CommentId { get; set; }

        public override string Validate()
        {
            if (GrowthRecordDetailId <= 0 || CommentId <= 0)
            {
                return "数据校验不合法";
            }
            return string.Empty;
        }
    }
}

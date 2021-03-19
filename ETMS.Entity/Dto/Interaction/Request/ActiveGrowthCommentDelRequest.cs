using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthCommentDelRequest : RequestBase
    {
        public long CommentId { get; set; }

        public long GrowthRecordId { get; set; }

        public override string Validate()
        {
            if (CommentId <= 0 || GrowthRecordId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}

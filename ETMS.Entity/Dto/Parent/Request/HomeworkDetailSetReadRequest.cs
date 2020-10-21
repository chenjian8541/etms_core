using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class HomeworkDetailSetReadRequest : ParentRequestBase
    {
        public long HomeworkDetailId { get; set; }

        public override string Validate()
        {
            if (HomeworkDetailId <= 0)
            {
                return "数据校验不合法";
            }
            return string.Empty;
        }
    }
}

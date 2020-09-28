using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentDetailInfoRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "数据校验不合法";
            }
            return string.Empty;
        }
    }
}

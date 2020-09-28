using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentTrackLogGetLastRequest : RequestBase
    {
        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据不合法";
            }
            return base.Validate();
        }
    }
}

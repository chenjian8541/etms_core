using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentTrackLogGetListRequest : RequestBase
    {
        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}

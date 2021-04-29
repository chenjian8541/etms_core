using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentLeaveApplyLogGetRequest : RequestBase
    {
        public long StudentLeaveApplyLogId { get; set; }

        public override string Validate()
        {
            if (StudentLeaveApplyLogId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}

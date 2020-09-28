using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentLeaveApplyHandleRequest : RequestBase
    {
        public long StudentLeaveApplyLogId { get; set; }

        public byte NewHandleStatus { get; set; }

        public string HandleRemark { get; set; }

        public override string Validate()
        {
            if (StudentLeaveApplyLogId <= 0)
            {
                return "请求数据不合法";
            }
            if (NewHandleStatus <= 0)
            {
                return "请选择处理结果";
            }
            return string.Empty;
        }
    }
}

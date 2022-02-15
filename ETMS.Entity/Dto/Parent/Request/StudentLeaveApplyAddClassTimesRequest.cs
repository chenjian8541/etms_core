using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentLeaveApplyAddClassTimesRequest : StudentLeaveApplyRequest
    {
        public long ClassTimesId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择请假人";
            }
            if (ClassTimesId <= 0)
            {
                return "请选择课次";
            }
            if (string.IsNullOrEmpty(LeaveContent))
            {
                return "请输入请假事由";
            }
            return string.Empty;
        }
    }
}

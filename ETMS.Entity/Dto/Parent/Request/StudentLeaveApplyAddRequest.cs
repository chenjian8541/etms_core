using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentLeaveApplyAddRequest : ParentRequestBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public string LeaveContent { get; set; }

        public long StudentId { get; set; }

        public override string Validate()
        {
            if (EndDate < StartDate)
            {
                return "请假日期格式不正确";
            }
            if (StartTime < 0 || EndTime <= 0)
            {
                return "请假时间格式不正确";
            }
            if (StartDate == EndDate && StartTime >= EndTime)
            {
                return "请假时间格式不正确";
            }
            if ((EndDate - StartDate).TotalDays > 60)
            {
                return "请假时长不能超过60天";
            }
            if (string.IsNullOrEmpty(LeaveContent))
            {
                return "请输入请假事由";
            }
            return string.Empty;
        }
    }
}

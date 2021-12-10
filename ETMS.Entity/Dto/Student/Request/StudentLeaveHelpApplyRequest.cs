using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentLeaveHelpApplyRequest : RequestBase
    {
        public long StudentId { get; set; }

        public List<string> Ot { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string LeaveContent { get; set; }

        public List<string> LeaveMediasKeys { get; set; }

        public string HandleRemark { get; set; }
        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择请假学员";
            }
            if (Ot == null || Ot.Count != 2)
            {
                return "请选择请假时间";
            }
            StartTime = Convert.ToDateTime(Ot[0]);
            EndTime = Convert.ToDateTime(Ot[1]);
            if (StartTime >= EndTime)
            {
                return "请假时间格式不正确";
            }
            if ((EndTime - StartTime).TotalDays > 60)
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
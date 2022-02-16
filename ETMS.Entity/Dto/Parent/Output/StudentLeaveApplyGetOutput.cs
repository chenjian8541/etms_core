using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentLeaveApplyGetOutput
    {
        public long Id { get; set; }

        public string TitleDesc { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 请假理由
        /// </summary>
        public string LeaveContent { get; set; }

        /// <summary>
        /// 请假备注
        /// </summary>
        public string LeaveRemark { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentLeaveApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public string HandleStatusDesc { get; set; }

        public DateTime ApplyOt { get; set; }
    }
}

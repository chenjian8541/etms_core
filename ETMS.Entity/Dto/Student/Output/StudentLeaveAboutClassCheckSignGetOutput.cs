using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentLeaveAboutClassCheckSignGetOutput
    {
        public long StudentId { get; set; }

        public string LeaveDesc { get; set; }

        /// <summary>
        /// 请假理由
        /// </summary>
        public string LeaveContent { get; set; }
    }
}

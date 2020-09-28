using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ProcessClassTimesRuleAddOfTimeRequest: RequestBase
    {
        public long ClassId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 每周几上课
        /// </summary>
        public List<int> Weeks { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<long> TeacherIds { get; set; }

        public List<long> CourseIds { get; set; }

        public string ClassContent { get; set; }

        public bool IsJumpHoliday { get; set; }
    }
}

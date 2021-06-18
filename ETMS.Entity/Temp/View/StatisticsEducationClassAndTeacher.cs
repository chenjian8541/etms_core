using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    public class StatisticsEducationClassAndTeacher
    {
        public string CourseList { get; set; }

        public long ClassId { get; set; }

        public string Teachers { get; set; }

        public decimal TotalClassTimes { get; set; }

        public decimal TotalDeSum { get; set; }

        public int TotalCount { get; set; }

        public int TotalNeedAttendNumber { get; set; }

        public int TotalAttendNumber { get; set; }
    }
}

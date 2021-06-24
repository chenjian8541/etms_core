using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsEducationClassMonthGetPagingOutput
    {
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public string Attendance { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsEducationStudentMonthGetPagingOutput
    {
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int BeLateCount { get; set; }

        public int LeaveCount { get; set; }

        public int NotArrivedCount { get; set; }
    }
}

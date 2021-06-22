using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class StatisticsEducationCourseMonthGetPagingOutput
    {
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public decimal Attendance { get; set; }
    }
}

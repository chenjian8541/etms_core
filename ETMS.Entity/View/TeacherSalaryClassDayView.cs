using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TeacherSalaryClassDayView
    {
        public long TeacherId { get; set; }

        public long ClassId { get; set; }

        public decimal TotalTeacherClassTimes { get; set; }

        public decimal TotalStudentClassTimes { get; set; }

        public decimal TotalDeSum { get; set; }

        public int TotalArrivedAndBeLateCount { get; set; }

        public int TotalArrivedCount { get; set; }

        public int TotalBeLateCount { get; set; }

        public int TotalLeaveCount { get; set; }

        public int TotalNotArrivedCount { get; set; }

        public int TotalTryCalssStudentCount { get; set; }

        public int TotalMakeUpStudentCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    public class StatisticsEducationStudentView
    {
        public long StudentId { get; set; }

        public decimal TotalDeClassTimes { get; set; }

        public decimal TotalDeSum { get; set; }

        public int TotalCount { get; set; }
    }
}

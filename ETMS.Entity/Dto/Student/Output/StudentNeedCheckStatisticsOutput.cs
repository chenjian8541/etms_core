using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentNeedCheckStatisticsOutput
    {
        public int NeedCheckInCount { get; set; }

        public int NeedCheckOutCount { get; set; }

        public int NeedAttendClassCount { get; set; }

        public bool IsShowCheckOut { get; set; }

        public bool IsShowAttendClass { get; set; }
    }
}

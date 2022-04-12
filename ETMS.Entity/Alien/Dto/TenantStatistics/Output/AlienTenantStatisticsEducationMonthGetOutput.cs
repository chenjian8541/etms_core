using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlienTenantStatisticsEducationMonthGetOutput
    {
        public decimal TeacherTotalClassTimes { get; set; }

        public int TeacherTotalClassCount { get; set; }

        public decimal TotalDeSum { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public string Attendance { get; set; }
    }
}

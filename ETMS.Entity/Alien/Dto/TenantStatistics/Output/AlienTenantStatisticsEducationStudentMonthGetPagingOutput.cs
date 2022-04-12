using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlienTenantStatisticsEducationStudentMonthGetPagingOutput
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

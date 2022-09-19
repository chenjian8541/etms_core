using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCheckOnStatisticsGetPagingOutput
    {
        public int CheckInCount { get; set; }

        public int CheckOutCount { get; set; }

        public int CheckAttendClassCount { get; set; }

        public string OtDesc { get; set; }
    }
}

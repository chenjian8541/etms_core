using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantStatisticsStudentCountMonthPagingGetOutput
    {
        public long Id { get; set; }

        public int AddStudentCount { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class GetStatisticsStudentCountMonthPagingOutput
    {
        public long Id { get; set; }

        public int AddStudentCount { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}

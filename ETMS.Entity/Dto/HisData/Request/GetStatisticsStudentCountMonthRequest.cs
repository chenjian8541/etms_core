using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class GetStatisticsStudentCountMonthRequest : RequestBase
    {
        public int? Year { get; set; }
    }
}

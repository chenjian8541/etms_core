using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class StatisticsEducationMonthGetRequest : RequestBase
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public override string Validate()
        {
            if (Year <= 0 || Month <= 0)
            {
                return "请选择月份";
            }
            return string.Empty;
        }
    }
}

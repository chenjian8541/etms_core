using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class StatisticsClassAttendanceRequest : RequestBase
    {
        public string Ot { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Ot))
            {
                return "请选择时间";
            }
            return string.Empty;
        }
    }
}


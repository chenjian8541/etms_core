using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckOnStatisticsGetPagingRequest : RequestPagingBase
    {
        public DateTime? CheckOt { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (CheckOt != null)
            {
                condition.Append($" AND Ot = '{CheckOt.EtmsToDateString()}'");
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (CheckOt != null && CheckOt.Value > DateTime.Now.Date)
            {
                return "考勤日期不能大于当前日期";
            }
            return base.Validate();
        }
    }
}

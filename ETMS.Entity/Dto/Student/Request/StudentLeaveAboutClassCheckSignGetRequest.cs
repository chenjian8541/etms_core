using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentLeaveAboutClassCheckSignGetRequest : RequestBase
    {
        public DateTime? Ot { get; set; }

        /// <summary>
        /// 上课时间 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 上课时间  结束
        /// </summary>
        public int EndTime { get; set; }

        public override string Validate()
        {
            if (Ot == null)
            {
                return "请选择上课日期";
            }
            if (StartTime == 0)
            {
                return "请选择上课开始时间";
            }
            if (EndTime == 0)
            {
                return "请选择上课结束时间";
            }
            if (StartTime >= EndTime)
            {
                return "上课结束时间必须大于开始时间";
            }
            return string.Empty;
        }
    }
}
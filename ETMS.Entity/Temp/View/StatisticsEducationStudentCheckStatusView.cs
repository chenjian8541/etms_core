using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    public class StatisticsEducationStudentCheckStatusView
    {
        public long StudentId { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public int TotalCount { get; set; }
    }
}

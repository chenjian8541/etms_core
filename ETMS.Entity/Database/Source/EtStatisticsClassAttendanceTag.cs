using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 考勤结果标记统计
    /// </summary>
    [Table("EtStatisticsClassAttendanceTag")]
    public class EtStatisticsClassAttendanceTag : Entity<long>
    {
        public DateTime Ot { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public int Count { get; set; }
    }
}

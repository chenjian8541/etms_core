using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 教务数据-出勤率
    /// </summary>
    [Table("EtStatisticsClassAttendance")]
    public class EtStatisticsClassAttendance : Entity<long>
    {
        public long ClassRecordId { get; set; }

        public DateTime Ot { get; set; }

        public int Day { get; set; }

        public int StartTime { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public decimal Attendance { get; set; }

    }
}

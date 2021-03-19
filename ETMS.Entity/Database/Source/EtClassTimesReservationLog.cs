using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 约课记录
    /// </summary>
    [Table("EtClassTimesReservationLog")]
    public class EtClassTimesReservationLog : Entity<long>
    {
        public long ClassId { get; set; }

        public long CourseId { get; set; }

        public long ClassTimesId { get; set; }

        public long RuleId { get; set; }

        public long StudentId { get; set; }

        public byte Week { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndTime { get; set; }

        public DateTime ClassOt { get; set; }

        public DateTime CreateOt { get; set; }

        /// <summary>
        ///  状态  <see cref=" ETMS.Entity.Enum.EmClassTimesReservationLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string Remark { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员上课通知
    /// </summary>
    [Table("EtTempStudentClassNotice")]
    public class EtTempStudentClassNotice: Entity<long>
    {
        public long ClassTimesId { get; set; }

        /// <summary>
        /// 上课日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassTime { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EmTempStudentClassNoticeStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}

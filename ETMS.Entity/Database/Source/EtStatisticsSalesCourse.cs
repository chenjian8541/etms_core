using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 销售的课程统计
    /// </summary>
    [Table("EtStatisticsSalesCourse")]
    public class EtStatisticsSalesCourse : Entity<long>
    {
        public long CourseId { get; set; }

        public DateTime Ot { get; set; }

        public int Count { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// 购买单位  <see cref="ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte BugUnit { get; set; }
    }
}

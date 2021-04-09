using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 教务数据-课程
    /// </summary>
    [Table("EtStatisticsClassCourse")]
    public class EtStatisticsClassCourse : Entity<long>
    {
        public DateTime Ot { get; set; }

        public long CourseId { get; set; }

        public decimal ClassTimes { get; set; }
    }
}

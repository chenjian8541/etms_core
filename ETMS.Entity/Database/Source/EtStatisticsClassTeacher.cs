using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 教务数据-老师
    /// </summary>
    [Table("EtStatisticsClassTeacher")]
    public class EtStatisticsClassTeacher : Entity<long>
    {
        public DateTime Ot { get; set; }

        public long TeacherId { get; set; }

        public int ClassTimes { get; set; }
    }
}

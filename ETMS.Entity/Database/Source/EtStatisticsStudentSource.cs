using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员来源
    /// </summary>
    [Table("EtStatisticsStudentSource")]
    public class EtStatisticsStudentSource : Entity<long>
    {
        public long? SourceId { get; set; }

        public DateTime Ot { get; set; }

        public int Count { get; set; }
    }
}

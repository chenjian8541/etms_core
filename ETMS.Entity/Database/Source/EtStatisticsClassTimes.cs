using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 教务数据-课时统计
    /// </summary>
    [Table("EtStatisticsClassTimes")]
    public class EtStatisticsClassTimes : Entity<long>
    {
        public DateTime Ot { get; set; }

        public decimal ClassTimes { get; set; }

        public decimal DeSum { get; set; }
    }
}

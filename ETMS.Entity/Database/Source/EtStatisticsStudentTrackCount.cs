using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 跟进次数
    /// </summary>
    [Table("EtStatisticsStudentTrackCount")]
    public class EtStatisticsStudentTrackCount : Entity<long>
    {
        /// <summary>
        /// 跟进次数
        /// </summary>
        public int TrackCount { get; set; }

        /// <summary>
        /// 时间(日期)
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

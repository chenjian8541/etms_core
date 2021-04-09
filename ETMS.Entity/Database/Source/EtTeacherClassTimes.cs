using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 老师课时统计
    /// </summary>
    [Table("EtTeacherClassTimes")]
    public class EtTeacherClassTimes : Entity<long>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 一号时间
        /// </summary>
        public DateTime FirstTime { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public decimal ClassTimes { get; set; }

        /// <summary>
        /// 课次
        /// </summary>
        public int ClassCount { get; set; }

    }
}

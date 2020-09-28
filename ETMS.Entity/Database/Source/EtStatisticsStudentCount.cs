using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员新增数量
    /// </summary>
    [Table("EtStatisticsStudentCount")]
    public class EtStatisticsStudentCount : Entity<long>
    {
        /// <summary>
        /// 增加学员人数
        /// </summary>
        public int AddStudentCount { get; set; }

        /// <summary>
        /// 时间(日期)
        /// </summary>
        public DateTime Ot { get; set; }
    }
}

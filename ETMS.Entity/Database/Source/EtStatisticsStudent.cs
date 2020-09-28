using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsStudent")]
    public class EtStatisticsStudent : Entity<long>
    {
        /// <summary>
        /// 数据类别   <see cref="ETMS.Entity.Enum.EmStatisticsStudentType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string ContentData { get; set; }
    }
}

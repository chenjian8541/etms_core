using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsFinanceIncomeMonth")]
    public class EtStatisticsFinanceIncomeMonth : Entity<long>
    {
        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 项目名称  <see cref=" ETMS.Entity.Enum.EmIncomeLogProjectType"/>
        /// </summary>
        public long ProjectType { get; set; }

        public int TotalCount { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalSum { get; set; }
    }
}

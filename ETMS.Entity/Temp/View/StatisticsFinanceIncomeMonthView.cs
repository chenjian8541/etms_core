using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp.View
{
    public class StatisticsFinanceIncomeMonthView
    {
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

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class StatisticsFinanceIncomeView
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 项目名称  <see cref=" ETMS.Entity.Enum.EmIncomeLogProjectType"/>
        /// </summary>
        public long ProjectType { get; set; }

        /// <summary>
        /// 支付方式 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalSum { get; set; }
    }
}

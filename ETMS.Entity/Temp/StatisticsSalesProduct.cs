using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class StatisticsSalesProduct
    {
        public decimal DaySum { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmOrderProductType"/>
        /// </summary>
        public byte ProductType { get; set; }
    }
}

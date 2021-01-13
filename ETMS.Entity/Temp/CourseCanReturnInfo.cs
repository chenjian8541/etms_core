using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class CourseCanReturnInfo
    {
        /// <summary>
        /// 剩余数量(课时/天)
        /// </summary>
        public decimal SurplusQuantity { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public bool IsHas { get; set; }

        /// <summary>
        /// 按月的  以天作为单价
        /// </summary>
        public decimal Price { get; set; }

        public int BuyValidSmallQuantity { get; set; }
    }
}

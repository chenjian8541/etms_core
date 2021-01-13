using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseHasDetailGetOutput
    {
        public long OrderDetailId { get; set; }

        public string OrderNo { get; set; }

        public long ProductId { get; set; }

        public string ProductTypeDesc { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public byte BugUnit { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public int GiveQuantity { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 剩余数量(课时/天)
        /// </summary>
        public string SurplusQuantity { get; set; }

        public string SurplusQuantityDesc { get; set; }

        /// <summary>
        /// 按月的  以天作为单价
        /// </summary>
        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public decimal ItemAptSum { get; set; }
        public decimal ItemSum { get; set; }

        public int BuyValidSmallQuantity { get; set; }
    }
}

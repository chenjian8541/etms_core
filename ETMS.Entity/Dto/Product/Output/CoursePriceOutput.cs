using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CoursePriceOutput
    {
        /// <summary>
        /// 收费类型 <see cref="ETMS.Entity.Enum.EmCoursePriceType"/>
        /// </summary>
        public byte PriceType { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Price { get; set; }
    }
}

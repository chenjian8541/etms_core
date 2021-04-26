using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class CoursePriceRuleItem : IValidate
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Price { get; set; }

        public string Points { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "[收费标准]名称不能为空";
            }
            if (Quantity <= 0)
            {
                return "[收费标准]数量必须大于0";
            }
            if (TotalPrice < 0)
            {
                return "[收费标准]总价不能小于0";
            }
            return string.Empty;
        }
    }
}

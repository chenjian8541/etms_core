using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class CoursePriceRuleOutput
    {
        public bool IsByClassTimes { get; set; }

        public List<CoursePriceRuleOutputItem> ByClassTimes { get; set; }

        public bool IsByMonth { get; set; }

        public List<CoursePriceRuleOutputItem> ByMonth { get; set; }
    }

    public class CoursePriceRuleOutputItem
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Price { get; set; }
    }
}

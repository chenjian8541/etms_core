using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class MallCartView
    {
        public List<MallCartItem> CartItems { get; set; }
    }

    public class MallCartItem
    {

        public long Id { get; set; }

        public string GId { get; set; }

        public int BuyCount { get; set; }

        public long? CoursePriceRuleId { get; set; }

        public string CoursePriceRuleDesc { get; set; }

        public decimal TotalPrice { get; set; }

        public int TotalPoint { get; set; }

        public List<ParentBuyMallGoodsSubmitSpecItem> SpecItems { get; set; }
    }
}

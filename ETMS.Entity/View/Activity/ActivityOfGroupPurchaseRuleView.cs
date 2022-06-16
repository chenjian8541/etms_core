using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.Activity
{
    public class ActivityOfGroupPurchaseRuleContentView
    {
        public List<ActivityOfGroupPurchaseRuleItem> Item { get; set; }
    }

    public class ActivityOfGroupPurchaseRuleItem
    {
        public int LimitCount { get; set; }

        public decimal Money { get; set; }
    }
}

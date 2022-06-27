using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniActivityGroupPurchaseDiscountOutput
    {
        public WxMiniActivityHomeJoinRoute JoinRoute { get; set; }

        public bool IsMultiGroupPurchase { get; set; }

        public WxMiniActivityHomeGroupPurchaseRule GroupPurchaseRule { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniGroupPurchaseStartGoProcessOutput
    {
        public bool IsMustPay { get; set; }

        public long ActivityRouteItemId { get; set; }

        public WxMiniGroupPurchasePayInfo PayInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniGroupPurchaseJoinProcessOutput
    {
        public bool IsMustPay { get; set; }

        public WxMiniGroupPurchasePayInfo PayInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniGroupPurchasePayInfo
    {
        public long routeItemId { get; set; }

        public string orderNo { get; set; }

        public string appId { get; set; }

        public string timeStamp { get; set; }

        public string nonceStr { get; set; }

        public string package_str { get; set; }

        public string signType { get; set; }

        public string paySign { get; set; }

        public string uuid { get; set; }

        public string token_id { get; set; }
    }
}

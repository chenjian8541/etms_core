using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Request
{
    public class UnifiedOrderRequest
    {
        public DateTime Now { get; set; }

        public long PayLogId { get; set; }

        public string OrderBody { get; set; }

        public string SubAppid { get; set; }

        public string OpenId { get; set; }

        public string OrderNo { get; set; }

        /// <summary>
        /// 支付金额（元）
        /// </summary>
        public decimal PayMoney { get; set; }

        /// <summary>
        /// 支付金额（分）
        /// </summary>
        public int PayMoneyCent { get; set; }


        public string IpAddress { get; set; }

    }
}

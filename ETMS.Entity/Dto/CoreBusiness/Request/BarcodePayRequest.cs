using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Request
{
    public class BarcodePayRequest
    {
        public DateTime Now { get; set; }

        public string OrderNo { get; set; }

        public string AuthNo { get; set; }

        public string OrderDesc { get; set; }

        /// <summary>
        /// 支付金额（元）
        /// </summary>
        public decimal PayMoney { get; set; }

        /// <summary>
        /// 支付金额（分）
        /// </summary>
        public int PayMoneyCent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class BarCodePayOutput
    {
        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int PayStatus { get; set; }

        public long LcsAccountId { get; set; }

        public string OrderNo { get; set; }

        public string pay_type { get; set; }

        public string out_trade_no { get; set; }
    }
}

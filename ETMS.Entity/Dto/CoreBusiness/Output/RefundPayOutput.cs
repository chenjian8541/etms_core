using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Output
{
    public class RefundPayOutput : AgtPayServiceOutputBase
    {
        /// <summary>
        /// 状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogStatus"/>
        /// </summary>
        public int RefundStatus { get; set; }

        public string out_refund_no { get; set; }

        public string refund_fee { get; set; }
    }
}

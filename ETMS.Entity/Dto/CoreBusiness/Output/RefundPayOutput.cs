using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Output
{
    public class RefundPayOutput : AgtPayServiceOutputBase
    {
        public string out_refund_no { get; set; }

        public string refund_fee { get; set; }
    }
}

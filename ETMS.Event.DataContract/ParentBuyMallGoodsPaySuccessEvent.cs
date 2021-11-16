using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ParentBuyMallGoodsPaySuccessEvent : Event
    {
        public ParentBuyMallGoodsPaySuccessEvent(int tenantId) : base(tenantId)
        { }

        public long LcsPayLogId { get; set; }

        public DateTime Now { get; set; }
    }
}

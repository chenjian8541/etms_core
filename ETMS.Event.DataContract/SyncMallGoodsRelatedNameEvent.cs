using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncMallGoodsRelatedNameEvent : Event
    {
        public SyncMallGoodsRelatedNameEvent(int tenantId) : base(tenantId)
        {
        }

        public byte ProductType { get; set; }

        public long RelatedId { get; set; }

        public string NewName { get; set; }
    }
}

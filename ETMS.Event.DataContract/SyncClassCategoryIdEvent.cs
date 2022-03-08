using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SyncClassCategoryIdEvent : Event
    {
        public SyncClassCategoryIdEvent(int tenantId) : base(tenantId)
        { }

        public long ClassId { get; set; }

        public long? NewClassCategoryId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class CloudStorageAnalyzeEvent : Event
    {
        public CloudStorageAnalyzeEvent(int tenantId) : base(tenantId)
        {
        }

        public int AgentId { get; set; }
    }
}

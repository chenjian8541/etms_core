using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.ZhuTong.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class SmsBatchSendEvent : Event
    {
        public SmsBatchSendEvent(int tenantId) : base(tenantId)
        {
        }

        public SendSmsPaRes SendSmsPaRes { get; set; }

        public SmsBatchSendRequest SmsBatchSendRequest { get; set; }

        public DateTime SendTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Request
{
    public class SmsBatchSendRequest : SmsBase
    {
        public SmsBatchSendRequest(int tenantId) : base(tenantId) { }

        public long UserId { get; set; }

        public List<SmsBatchSendItem> SmsBatch { get; set; }
    }

    public class SmsBatchSendItem
    {
        public long StudentId { get; set; }

        public string Phone { get; set; }

        public string SmsContent { get; set; }
    }
}

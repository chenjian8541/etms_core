using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TenantEntrancePCView
    {
        public int TenantId { get; set; }

        public long UserId { get; set; }

        public long NowTimestamp { get; set; }
    }
}

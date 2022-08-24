using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class UpdateTenantIpAddressEvent: Event
    {
        public UpdateTenantIpAddressEvent(int tenantId) : base(tenantId)
        {
        }

        public string IpAddress { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class CloudFileAutoDelDayEvent : Event
    {
        public CloudFileAutoDelDayEvent(int tenantId) : base(tenantId)
        {
        }
        public DateTime DelDate { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTenantCloudStorageType"/>
        /// </summary>
        public string FileTag { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class TenantTxCloudUCountEvent : Event
    {
        public TenantTxCloudUCountEvent(int tenantId) : base(tenantId)
        { }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantTxCloudUCountType"/>
        /// </summary>
        public byte Type { get; set; }

        public int AddUseCount { get; set; }

        public long StudentId { get; set; }
    }
}


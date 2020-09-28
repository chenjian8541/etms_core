using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    /// <summary>
    /// 同步班级信息
    /// </summary>
    public class SyncClassInfoEvent : Event
    {
        public SyncClassInfoEvent(int tenantId, long classId) : base(tenantId)
        {
            this.ClassId = classId;
        }

        public long ClassId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Activity
{
    public class SyncActivityBehaviorCountEvent : Event
    {
        public SyncActivityBehaviorCountEvent(int tenantId) : base(tenantId)
        { }

        public long ActivityId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string OpenId { get; set; }

        public byte BehaviorType { get; set; }
    }

    public struct ActivityBehaviorType
    {
        /// <summary>
        /// 访问
        /// </summary>
        public const byte Access = 0;

        /// <summary>
        /// 转发
        /// </summary>
        public const byte Retweet = 1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class AutoCheckSignClassTimesEvent : Event
    {
        public AutoCheckSignClassTimesEvent(int tenantId) : base(tenantId)
        {
        }

        public long ClassTimesId { get; set; }

        /// <summary>
        /// 补课是否扣课时
        /// </summary>
        public bool MakeupIsDeClassTimes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Statistics
{
    public class SysTenantStatistics2Event : Event
    {
        public SysTenantStatistics2Event(int tenantId) : base(tenantId)
        { }

        /// <summary>
        /// <see cref="SysTenantStatistics2Type"/>
        /// </summary>
        public int Type { get; set; }
    }

    public struct SysTenantStatistics2Type
    {
        public const int Com = 0;

        public const int StudentCourse = 1;
    }
}

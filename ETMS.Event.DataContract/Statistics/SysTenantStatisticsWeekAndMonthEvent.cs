using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract.Statistics
{
    public class SysTenantStatisticsWeekAndMonthEvent : Event
    {
        public SysTenantStatisticsWeekAndMonthEvent(int tenantId) : base(tenantId)
        { }

        public SysTenantStatisticsWeekAndMonthEvent(int tenantId, int type) : base(tenantId)
        {
            this.Type = type;
        }

        /// <summary>
        /// <see cref="StatisticsWeekAndMonthType"/>
        /// </summary>
        public int Type { get; set; }
    }

    public struct StatisticsWeekAndMonthType
    {
        public const int ALL = 0;

        public const int Income = 1;

        public const int ClassTimes = 2;

        public const int BuyCourse = 3;
    }
}
